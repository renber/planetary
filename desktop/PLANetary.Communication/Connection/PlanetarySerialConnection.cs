using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using PLANetary.Core.Connection;

using proto = PLANetary.Communication.Protobuf;
using pl = PLANetary.Core.Types;

namespace PLANetary.Communication.Connection
{

    /// <summary>
    /// Class which handles the communication with a connected PLANet board through a serial port
    /// </summary>
    public class PlanetarySerialConnection : AbstractPlanetaryConnection
    {

        #region Variables

        SerialPort sPort = null;     

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value which indicates if the interface is connected to a serial port
        /// </summary>
        public override bool IsConnected
        {
            get
            {
                return sPort != null && sPort.IsOpen;
            }
        }

        public String PortName
        {
            get
            {
                return (sPort == null || !sPort.IsOpen) ? "" : sPort.PortName;
            }
        }

        public override string ConnectionDescriptor => PortName;

        #endregion

        #region Connection management

        /// <summary>
        /// Connect to the given serial port
        /// </summary>
        /// <param name="serialPortName"></param>
        /// <returns></returns>
        public override bool Connect(IPlanetaryConnectionParameters parameters)
        {
            if (!(parameters is SerialConnectionParameters))
            {
                throw new ArgumentException("Parameters have to be of type SerialConnectionParameters");
            }

            String serialPortName = ((SerialConnectionParameters)parameters).PortName;

            if (IsConnected)
                if (serialPortName == sPort.PortName)
                    return true;
                else
                    return false;

            try
            {
                PendingQueries.Clear();

                sPort = new SerialPort(serialPortName);
                sPort.BaudRate = 115200; // default PLANet baud rate
                sPort.StopBits = StopBits.One;
                sPort.Parity = Parity.None;

                sPort.Open();
                
                if (!CheckIfDevicePresentIsPlanet())
                {
                    Disconnect();
                    return false;
                }                

                sPort.DataReceived += new SerialDataReceivedEventHandler(sPort_DataReceived);                

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// CHecks if the connected device is really a PLANet sink
        /// </summary>
        /// <returns></returns>
        private bool CheckIfDevicePresentIsPlanet()
        {
            int oldWTimeout = sPort.WriteTimeout;
            int oldTimeout = sPort.ReadTimeout;            

            try
            {                                
                // send the "Are you a PLANet?" command and connect desire
                SendCommand("PLN?");
                
                // get response            
                sPort.ReadTimeout = 4000;
                string inStr = sPort.ReadTo("\r");

                return inStr == "YES";
            }
            catch (TimeoutException)
            {
                return false;
            }
            finally
            {
                sPort.WriteTimeout = oldWTimeout;
                sPort.ReadTimeout = oldTimeout;                
            }                       
        }        

        public override void Disconnect()
        {
            if (IsConnected)
            {                
                #if !DEBUG
                // send disconnect
                SendCommand("DSC!");
                #endif

                while (sPort.BytesToWrite > 0)
                    Thread.Sleep(10);

                sPort.Close();
                sPort.DataReceived -= sPort_DataReceived;
                sPort.Dispose();

                sPort = null;
            }
        }

        #endregion

        protected override void Write(byte[] data)
        {
            //sPort.WriteTimeout = 4000;
            sPort.Write(data, 0, data.Length);
        }

        #region Receiving

        void sPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            while (sPort.BytesToRead > 0)
                Read((byte)sPort.ReadByte());
        }

        #endregion               

        public override List<pl.Query> EnumerateRunningQueries()
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to sink.");

            List<pl.Query> rList = new List<pl.Query>();

            sPort.DataReceived -= sPort_DataReceived; // don't let the event steal our data!

            try
            {
                // demand the active queries
                SendCommand("LST!");
                sPort.ReadTimeout = 4000;
                int noQueries = 0;
                string response = sPort.ReadTo("\r");
                if (response.StartsWith("ActQ:")) // returns with the amount of running queries
                {
                    string s = response.Substring("ActQ:".Length);
                    noQueries = (byte)response[5];
                }

                for (int i = 0; i < noQueries; i++)
                {
                    byte[] preambleBuf = new byte[5];
                    sPort.Read(preambleBuf, 0, 5);

                    // check if preamble for query packet has been received
                    if (preambleBuf.Take(4).All(x => x == 255))
                    {
                        int pLen = preambleBuf[4];
                        byte[] pData = new byte[pLen];
                        sPort.Read(pData, 0, pLen);

                        var q = proto.Query.Parser.ParseFrom(pData);

                        // Todo
                        //rList.Add(q);
                    }
                }
            }
            catch (Exception exc)
            {

            }
            finally
            {
                sPort.DataReceived += sPort_DataReceived; // restore the event
            }

            return rList;
        }
    }

    public class SerialConnectionParameters : IPlanetaryConnectionParameters
    {
        public String PortName { get; set; }

        public SerialConnectionParameters(String portName)
        {
            PortName = portName;
        }
    }
}
