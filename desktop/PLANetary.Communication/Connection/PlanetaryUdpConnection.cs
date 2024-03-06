using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using PLANetary.Core.Connection;

using proto = PLANetary.Communication.Protobuf;
using pl = PLANetary.Core.Types;

namespace PLANetary.Communication.Connection
{
    public class PlanetaryUdpConnection : AbstractPlanetaryConnection
    {
        int AppPort = 32000;
        IPEndPoint nodeEndPoint;

        UdpClient client;

        UdpConnectionParameters udpParams;

        public override bool IsConnected => client != null;

        public override string ConnectionDescriptor => "Udp Port " + udpParams?.Port.ToString() ?? "n/a";

        public override bool Connect(IPlanetaryConnectionParameters parameters)
        {
            if (!(parameters is UdpConnectionParameters))
            {
                throw new ArgumentException("Parameters have to be of type UdpConnectionParameters");
            }

            udpParams = (UdpConnectionParameters)parameters;

            if (IsConnected)
                return true;

            PendingQueries.Clear();

            client = new UdpClient(AppPort);       
            nodeEndPoint =  new IPEndPoint(IPAddress.Parse(udpParams.Host), udpParams.Port); ;
            client.Connect(nodeEndPoint);

            if (!CheckIfDevicePresentIsPlanet())
            {
                // free the socket
                Disconnect();
                return false;
            }

            // start receiving
            DoReceive().ConfigureAwait(false);
            
            return true;
        }

        /// <summary>
        /// CHecks if the connected device is really a PLANet sink
        /// </summary>
        /// <returns></returns>
        private bool CheckIfDevicePresentIsPlanet()
        {            
            var oldTimeout = client.Client.ReceiveTimeout;

            try
            {
                // send the "Are you a PLANet?" command and connect desire
                SendCommand("PLN?");

                // get response                  
                client.Client.ReceiveTimeout = 6000;
                byte[] buf = client.Receive(ref nodeEndPoint);
                String response = Encoding.ASCII.GetString(buf.Skip(5).ToArray());

                return response == "YES";
            }
            catch (SocketException e)
            {
                return false;
            }
            catch (TimeoutException)
            {
                return false;
            } finally
            {
                client.Client.ReceiveTimeout = oldTimeout;
            }
        }

        public override void Disconnect()
        {
            if (client != null)
            {
                client.Close();
                client = null;
            }
        }

        public override List<pl.Query> EnumerateRunningQueries()
        {
            // TODO: not yet implemented
            return new List<pl.Query>();
        }

        private async Task DoReceive()
        {
            while (true)
            {
                var r = await client.ReceiveAsync();

                foreach(byte b in r.Buffer)
                    Read(b);
            }
        }

        protected override void Write(byte[] data)
        {
            client.Send(data, data.Length);
        }
    }

    public class UdpConnectionParameters : IPlanetaryConnectionParameters
    {
        public String Host { get; set; }

        public int Port { get; set; }

        public UdpConnectionParameters(String host, int port)
        {
            Host = host;
            Port = port;
        }
    }
}
