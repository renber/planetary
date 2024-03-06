using PLANetary.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLANetary.Core.Connection
{
    /// <summary>
    /// Interface for classes which allow to communicate with a PLANetary sink node
    /// </summary>
    public interface IPlanetaryConnection : IDisposable
    {
        /// <summary>
        /// Gets a value which indicates if the interface is connected to a sink node
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// A descriptor of the connection depenending on the actual implementation (e.g. SerialPort, Port, etc.)
        /// </summary>
        string ConnectionDescriptor { get; }

        /// <summary>
        /// Connect to a PLANetary sink node using the given parameters
        /// </summary>        
        bool Connect(IPlanetaryConnectionParameters parameters);

        /// <summary>
        /// Close an existing connection
        /// </summary>
        void Disconnect();

        /// <summary>
        /// get a list of all queries currently present in the network
        /// </summary>
        List<Query> EnumerateRunningQueries();

        /// <summary>
        /// Registers a query to receive its results (through the event QueryResultReceived)
        /// (does not send the query to the sink)
        /// </summary>        
        void RegisterQuery(Query q);

        /// <summary>
        /// Pushes the given query into the sensor network for execution
        /// The event ReceivedQueryResults is fired when results are received      
        /// </summary>        
        void SendQuery(Query query);

        /// <summary>
        /// Cancel the execution of the query with the given id
        /// </summary>
        void CancelQuery(int queryid);

        /// <summary>
        /// Cancel the execution of the given query
        /// </summary>
        void CancelQuery(Query query);

        // Event which is fired, when a query result packet has been received
        event EventHandler<QueryResultReceivedEventArgs> QueryResultReceived;
    }

    /// <summary>
    /// Marker interface for connection parameters
    /// </summary>
    public interface IPlanetaryConnectionParameters
    {

    }

    public class QueryResultReceivedEventArgs : EventArgs
    {
        public readonly uint QueryId;
        public readonly Resultset Results;

        public QueryResultReceivedEventArgs(uint queryID, Resultset results)
        {
            QueryId = queryID;
            Results = results;
        }
    }
}
