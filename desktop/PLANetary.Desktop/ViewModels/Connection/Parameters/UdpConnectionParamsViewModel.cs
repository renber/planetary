using PLANetary.Communication.Connection;
using PLANetary.Core.Connection;
using PLANetary.Core.Types;
using PLANetary.Views.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PLANetary.ViewModels.Connection
{
    class UdpConnectionParamsViewModel : ConnectionParamsViewModel
    {
               
        public override string Title => "UDP";

        protected String host;
        public String Host { get => host; set => ChangeProperty(ref host, value); }

        protected int port;
        public int Port { get => port; set => ChangeProperty(ref port, value); }        

        public UdpConnectionParamsViewModel()
        {
            ControlTemplate = new DataTemplate(typeof(UdpConnectionParamsViewModel));
            var spFactory = new FrameworkElementFactory(typeof(UdpConnectionParamsControl));
            ControlTemplate.VisualTree = spFactory;

            Host = "127.0.0.1";
            Port = 5000;
        }

        public override IPlanetaryConnection CreateConnectionInstance()
        {
            return new PlanetaryUdpConnection();
        }

        public override IPlanetaryConnectionParameters GetParameters()
        {
            if (String.IsNullOrEmpty(Host))
                throw new InvalidOperationException("No host given.");

            return new UdpConnectionParameters(Host, Port);
        }

        public override bool CanConnect()
        {
            return !String.IsNullOrEmpty(Host);
        }
    }
}
