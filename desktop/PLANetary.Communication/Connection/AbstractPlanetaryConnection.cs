using Google.Protobuf;
using PLANetary.Communication.Types;
using PLANetary.Core.Connection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using proto = PLANetary.Communication.Protobuf;
using pl = PLANetary.Core.Types;
using PLANetary.Communication.Protobuf;

namespace PLANetary.Communication.Connection
{
    /// <summary>
    /// Base class for IPlanetaryConnection implementatons which does most of the heavy-lifting
    /// and should be able to be used by most actual implementations
    /// </summary>
    public abstract class AbstractPlanetaryConnection : IPlanetaryConnection
    {
        #region Properties
        protected List<pl.Query> PendingQueries { get; } = new List<pl.Query>();

        public abstract bool IsConnected { get; }

        public abstract string ConnectionDescriptor { get; }
        #endregion

        #region Variables

        List<byte> currentReading = new List<byte>();
        bool nextReadingIsPacketLength = false;
        bool nowReadingPacket;
        int incomingPacketLength = -1;

        #endregion

        /// <summary>
        /// Write the given data to the underlying connection
        /// </summary>
        /// <param name="data">The data to write</param>
        protected abstract void Write(byte[] data);

        /// <summary>
        /// Inform the connection that a byte was read
        /// </summary>
        /// <param name="data"></param>
        protected void Read(byte b)
        {
            try
            {
                if (!nowReadingPacket)
                {
                    if (nextReadingIsPacketLength)
                    {
                        incomingPacketLength = b;

                        nextReadingIsPacketLength = false;

                        if (incomingPacketLength > 0)
                            nowReadingPacket = true;
                    }
                    else
                    {
                        currentReading.Add(b);

                        if (currentReading.Count == 4)
                        {
                            if (currentReading.All(c => c == 255))
                                nextReadingIsPacketLength = true;

                            currentReading.Clear();

                        }
                    }
                }
                else
                {
                    currentReading.Add(b);
                    if (currentReading.Count == incomingPacketLength)
                    {
                        // Packet finished
                        byte[] data = currentReading.ToArray();
                        ReadPacket(data);                        

                        nowReadingPacket = false;
                        currentReading.Clear();
                    }
                }
            }
            catch (Exception exc)
            {
                // --
            }
        }

        #region ConnectionManagement

        public abstract bool Connect(IPlanetaryConnectionParameters parameters);

        public abstract void Disconnect();

        #endregion

        #region Communication

        protected bool SendCommand(string cmd)
        {
            try
            {
                byte[] writeBuf = new byte[5 + cmd.Length];
                // preamble
                writeBuf[0] = 255;
                writeBuf[1] = 255;
                writeBuf[2] = 255;
                writeBuf[3] = 255;
                writeBuf[4] = (byte)cmd.Length;
                // command
                for (int i = 0; i < cmd.Length; i++)
                {
                    writeBuf[5 + i] = (byte)cmd[i];
                }

                Write(writeBuf);

                return true;
            }
            catch (TimeoutException exc)
            {
                return false;
            }
        }

        protected void ReadPacket(byte[] packetData)
        {
            proto.PlanetaryMessage msg = proto.PlanetaryMessage.Parser.ParseFrom(packetData);

            switch (msg.PayloadCase)
            {
                case PlanetaryMessage.PayloadOneofCase.Resultset:
                    ReadQueryResult(msg.Resultset);
                    break;
            }
        }

        /// <summary>
        /// Reads the stream and converts it to a QueryResultset and its corresponding QueryID
        /// </summary>
        /// <param name="packetData"></param>
        protected void ReadQueryResult(Resultset resultset)
        {            
            // find the corresponding query
            pl.Query query = PendingQueries.FirstOrDefault(q => q.QueryId == resultset.QueryId.ShortId);

            if (query == null)
                return;            
            
            // query finished, if it is not periodic
            if (!query.Periodic)
                PendingQueries.Remove(query);            

            OnQueryResultReceived(resultset.QueryId.ShortId, resultset.ToModel());
        }       

        /// <summary>
        /// Cancel the execution of the query with the given id
        /// </summary>
        /// <param name="queryid"></param>
        public void CancelQuery(int queryid)
        {
            // send cancel request
            PlanetaryMessage msg = new PlanetaryMessage();
            msg.Cancelquery = new CancelQueryMessage() { QueryId = new QueryId() { ShortId = (uint)queryid } };

            SendMessage(msg);            

            var query = PendingQueries.FirstOrDefault(q => q.QueryId == queryid);
            if (query != null)
            {                
                PendingQueries.Remove(query);
            }
        }

        /// <summary>
        /// Cancel the execution of the given query
        /// </summary>
        public void CancelQuery(pl.Query query)
        {
            CancelQuery(query.QueryId);
        }

        /// <summary>
        /// Pushes the given query into the sensor network for execution
        /// The event ReceivedQueryResults is fired when results are received      
        /// </summary>
        /// <param name="query"></param>
        public void SendQuery(pl.Query query)
        {
            if (!IsConnected)
                throw new InvalidOperationException("A packet can only be sent when a connection has been established beforehand.");

            proto.PlanetaryMessage msg = new proto.PlanetaryMessage();
            msg.Broadcast = new proto.QueryBroadcast() { DistanceToSink = 0, ParentId = new proto.NodeId() { ShortId = 0 }, ParentIsPartOfQuery = true };
            msg.Broadcast.Query = query.ToMessage();

            if (msg.Broadcast.Query.QueryId == null)
            {
                throw new InvalidOperationException("The QueryID field has not been initialized.");
            }

            SendMessage(msg);
                        
            if (msg.Broadcast.Query.Actions.Any(x => x.ContentCase == (proto.Action.ContentOneofCase) proto.ActionType.Selector))
                PendingQueries.Add(query);
        }

        protected void SendMessage(PlanetaryMessage msg)
        {
            using (var mStream = new MemoryStream())
            {
                byte[] packetHeader = new byte[] { 255, 255, 255, 255, (byte)msg.CalculateSize() };
                mStream.Write(packetHeader, 0, packetHeader.Length);

                using (var cStream = new CodedOutputStream(mStream))
                {
                    msg.WriteTo(cStream);
                }

                Write(mStream.ToArray());
            }
        }

        #endregion

        #region Control functions

        public abstract List<pl.Query> EnumerateRunningQueries();

        /// <summary>
        /// Registers a query to receive its results (through the event QueryResultReceived)
        /// (does not send the query to the sink)
        /// </summary>
        /// <param name="q"></param>
        public void RegisterQuery(pl.Query q)
        {
            if (PendingQueries.Any(x => x.QueryId == q.QueryId))
                throw new Exception("The ID of the query (" + q.QueryId + ") is already in use.");

            PendingQueries.Add(q);
        }

        #endregion

        #region Events

        public event EventHandler<QueryResultReceivedEventArgs> QueryResultReceived;

        protected void OnQueryResultReceived(uint queryId, pl.Resultset results)
        {
            if (QueryResultReceived != null)
                QueryResultReceived(this, new QueryResultReceivedEventArgs(queryId, results));
        }

        #endregion    

        #region IDisposable implementation
        public void Dispose()
        {
            Disconnect();
        }
        #endregion
    }
}
