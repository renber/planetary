using PLANetary.Core.Connection;
using PLANetary.ViewModels.Connection;
using PLANetary.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PLANetary.Services
{
    class DefaultDialogService : IDialogService
    {
        public void ShowErrorMessage(String message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public bool ShowConnectionDialog(out IPlanetaryConnection connection)
        {
            ConnectViewModel cvm = new ConnectViewModel(this);

            ConnectWindow connectWindow = new ConnectWindow();
            connectWindow.DataContext = cvm;
            connectWindow.Owner = Application.Current.MainWindow;
            connectWindow.ShowDialog();

            connection = cvm.Result;
            return connection != null;
        }
    }
}
