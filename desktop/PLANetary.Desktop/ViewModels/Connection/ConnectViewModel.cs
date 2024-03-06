using PLANetary.Core.Connection;
using PLANetary.Interaction;
using PLANetary.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PLANetary.ViewModels.Connection
{
    /// <summary>
    /// ViewModel which provides functionality to connect to a
    /// Planetary network
    /// </summary>
    class ConnectViewModel : WindowedViewModelBase
    {
        public IDialogService DialogService { get; }

        public ObservableCollection<ConnectionParamsViewModel> ConnectionTypes { get; } = new ObservableCollection<ConnectionParamsViewModel>();

        private ConnectionParamsViewModel selectedConnectionType;
        public ConnectionParamsViewModel SelectedConnectionType { get => selectedConnectionType; set => ChangeProperty(ref selectedConnectionType, value); }

        public IPlanetaryConnection Result { get; private set; }

        /// <summary>
        /// Connect to the com port given as parameter
        /// </summary>
        public ICommand ConnectCommand { get; private set; }

        public ICommand CancelCommand { get; private set; }

        public ConnectViewModel(IDialogService dialogService)
        {
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            // Add the connection types
            DiscoverConnectors();

#if DEBUG
            SelectedConnectionType = ConnectionTypes.FirstOrDefault(x => x.Title.Contains("NULL"));
#endif

            // Commands
            ConnectCommand = new RelayCommand(Connect, () => selectedConnectionType != null && selectedConnectionType.CanConnect());

            CancelCommand = new RelayCommand(() => { Result = null; RequestViewClose(); });
        }

        private void DiscoverConnectors()
        {
            ConnectionTypes.Clear();

            foreach (var connType in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!connType.IsAbstract && typeof(ConnectionParamsViewModel).IsAssignableFrom(connType))
                    ConnectionTypes.Add((ConnectionParamsViewModel)Activator.CreateInstance(connType));
            }
        }

        protected void Connect()
        {
            var connection = SelectedConnectionType.CreateConnectionInstance();

            // Connect
            if (!connection.Connect(SelectedConnectionType.GetParameters()))
            {                              
                DialogService.ShowErrorMessage("Could not connect to PLANet sink. Please make sure that you have selected the connection settings and that the node is in \'Sink\' mode.");
            }
            else
            {
                //QueryStatusViewModel.UpdateConnection(connection);
                //connection.QueryResultReceived += connection_QueryResultReceived;
                Result = connection;
                RequestViewClose();
            }
        }        

    }
}
