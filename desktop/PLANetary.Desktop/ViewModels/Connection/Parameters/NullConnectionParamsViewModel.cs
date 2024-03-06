using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PLANetary.Core.Connection;
using PLANetary.Core.Types;

namespace PLANetary.ViewModels.Connection
{
    class NullConnectionParamsViewModel : ConnectionParamsViewModel
    {
        public override string Title => "NULL interface";

        public override bool CanConnect()
        {
            return true;
        }

        public override IPlanetaryConnection CreateConnectionInstance()
        {
            return new NullConnection();
        }

        public override IPlanetaryConnectionParameters GetParameters()
        {
            return new NullConnectionparams();
        }
    }

    class NullConnectionparams : IPlanetaryConnectionParameters
    {

    }

    class NullConnection : IPlanetaryConnection
    {
        public bool IsConnected { get; private set; }

        public string ConnectionDescriptor => "NULL connection";

        public event EventHandler<QueryResultReceivedEventArgs> QueryResultReceived;

        public void CancelQuery(int queryid)
        {
            
        }

        public void CancelQuery(Query query)
        {
            
        }

        public bool Connect(IPlanetaryConnectionParameters parameters)
        {
            IsConnected = true;
            return true;
        }

        public void Disconnect()
        {
            IsConnected = false;
        }

        public void Dispose()
        {
            
        }

        public List<Query> EnumerateRunningQueries()
        {
            return new List<Query>();
        }

        public void RegisterQuery(Query q)
        {
            
        }

        public void SendQuery(Query query)
        {
            
        }
    }
}
