using PLANetary.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PLANetary.Views
{
    /// <summary>
    /// Interaktionslogik für ConnectWindow.xaml
    /// </summary>
    public partial class ConnectWindow : Window
    {
        public ConnectWindow()
        {
            InitializeComponent();

            
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property.Name == "DataContext")
            {
                if (e.OldValue is WindowedViewModelBase owvm)
                    owvm.OnViewCloseRequested -= OnViewCloseRequested;

                if (e.NewValue is WindowedViewModelBase nwvm)
                    nwvm.OnViewCloseRequested += OnViewCloseRequested;
            }
        }

        private void OnViewCloseRequested(object sender, EventArgs e)
        {
            Close();
        }
    }
}
