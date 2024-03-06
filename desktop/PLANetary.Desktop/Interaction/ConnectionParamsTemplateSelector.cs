using PLANetary.ViewModels.Connection;
using PLANetary.Views.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PLANetary.Interaction
{
    class ConnectionParamsTemplateSelector : DataTemplateSelector
    {

        public DataTemplate NoParamsTemplate { get; set; }

        public DataTemplate SerialPortDataTemplate { get; set; }

        public DataTemplate UdpDataTemplate { get; set; }

        public ConnectionParamsTemplateSelector()
        {
            NoParamsTemplate = new DataTemplate();
            FrameworkElementFactory spFactory = new FrameworkElementFactory(typeof(NoConnectionParamsControl));
            NoParamsTemplate.VisualTree = spFactory;                      
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {        
            if (item is ConnectionParamsViewModel cvm)
            {
                return cvm.ControlTemplate ?? NoParamsTemplate;
            }                           

            return NoParamsTemplate;
        }
    }
}
