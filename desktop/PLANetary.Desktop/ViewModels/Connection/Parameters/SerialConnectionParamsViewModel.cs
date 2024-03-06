using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PLANetary.Core.Types;
using System.Collections.ObjectModel;
using System.IO.Ports;
using PLANetary.Communication.Connection;
using PLANetary.Core.Connection;
using System.Windows;
using PLANetary.Views.Connection;

namespace PLANetary.ViewModels.Connection
{
    class SerialConnectionParamsViewModel : ConnectionParamsViewModel
    {
        /// <summary>
        /// The serial ports available on the system
        /// </summary>
        public ObservableCollection<string> SerialPorts { get; private set; }

        public override string Title => "Serial port";

        protected String selectedSerialPort;
        public String SelectedSerialPort { get => selectedSerialPort; set => ChangeProperty(ref selectedSerialPort, value); }        

        public SerialConnectionParamsViewModel()
        {
            ControlTemplate = new DataTemplate(typeof(SerialConnectionParamsViewModel));
            var spFactory = new FrameworkElementFactory(typeof(SerialConnectionParamsControl));
            ControlTemplate.VisualTree = spFactory;

            // get available serial Ports
            SerialPorts = new ObservableCollection<string>(SerialPort.GetPortNames());

            SelectedSerialPort = SerialPorts.FirstOrDefault();
        }

        public override IPlanetaryConnection CreateConnectionInstance()
        {
            return new PlanetarySerialConnection();
        }

        public override IPlanetaryConnectionParameters GetParameters()
        {
            if (SelectedSerialPort == null)
                throw new InvalidOperationException("No serial port selected.");

            return new SerialConnectionParameters(SelectedSerialPort);
        }

        public override bool CanConnect()
        {
            return SelectedSerialPort != null;
        }
    }
}
