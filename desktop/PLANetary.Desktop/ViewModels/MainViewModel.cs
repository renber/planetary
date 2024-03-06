using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using PLANetary.Core.Types;
using PLANetary.Interaction;
using System.IO.Ports;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Input;
using PLANetary.Extensions;
using PLANetary.ViewModels.Connection;
using System.Reflection;
using PLANetary.Communication.Connection;
using PLANetary.Core.Connection;
using PLANetaryQL;
using PLANetaryQL.Parser;
using PLANetaryQL.Parser.Exceptions;
using PLANetary.Services;
using PLANetary.ViewModels.Navigation;

namespace PLANetary.ViewModels
{
    class MainViewModel : ViewModelBase
    {

        #region Services

        IDialogService DialogService { get; }

        IChartDataFactory ChartDataFactory { get; }

        #endregion

        #region Variables

        IPlanetaryConnection connection;

        private IPlanetaryConnection Connection
        {
            get => connection;
            set
            {
                if (ChangeProperty(ref connection, value))
                {
                    OnPropertyChanged(nameof(IsConnected));
                    OnPropertyChanged(nameof(ConnectionStateStr));
                };
            }
        }

        Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        int lastQueryID = 0;

        #endregion

        #region Properties

        public ObservableCollection<QueryViewModel> ActiveQueries { get; } = new ObservableCollection<QueryViewModel>();
        public ObservableCollection<QueryViewModel> CompletedQueries { get; } = new ObservableCollection<QueryViewModel>();

        public QueryViewModel SelectedQuery
        {
            get
            {
                var sq = SelectedElement;
                return sq == null ? null : sq.DataContext as QueryViewModel;
            }
            set
            {
                var sq = FindNavigationForDataContext(value);
                if (sq != null)
                {
                    // expand the parent so that the child is visible
                    var parent = GetNavigationParent(sq);
                    if (parent != null)
                        parent.IsExpanded = true;

                    sq.IsSelected = true;
                    OnPropertyChanged();
                }
            }
        }

        public NavigationElement SelectedElement
        {
            get
            {
                var mainSel = Navigation.FirstOrDefault(x => x.IsSelected);
                if (mainSel != null)
                    return mainSel;

                // search one level deeper
                return Navigation.SelectMany(x => x.Items).FirstOrDefault(x => x.IsSelected);
            }
        }

        /// <summary>
        /// Describes the current connection state
        /// </summary>
        public String ConnectionStateStr
        {
            get
            {
                if (IsConnected)
                    return "Connected to PLANet sink over " + (Connection?.ConnectionDescriptor ?? "");
                else
                    return "Not Connected";
            }
        }

        /// <summary>
        /// Gets a value which indicates if a connection to a PLANet board has been established
        /// </summary>
        public bool IsConnected => Connection?.IsConnected ?? false;

        public ObservableCollection<NavigationElement> Navigation { get; } = new ObservableCollection<NavigationElement>();        

        #endregion        

        #region Commands        

        public ICommand ConnectCommand { get; }

        public ICommand DisconnectCommand { get; }    

        #endregion
        
        #region Constructor

        public MainViewModel(IDialogService dialogService, IChartDataFactory chartDataFactory)
        {
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            ChartDataFactory = chartDataFactory ?? throw new ArgumentNullException(nameof(chartDataFactory));

            CreateNavigation();            

            ConnectCommand = new RelayCommand(() =>
           {               
               if (DialogService.ShowConnectionDialog(out IPlanetaryConnection connection))
               {
                   Connection = connection;
                   Connection.QueryResultReceived += connection_QueryResultReceived;
               };
           }, () => !IsConnected);            

            DisconnectCommand = new RelayCommand(() =>            
            {
                Connection.QueryResultReceived -= connection_QueryResultReceived;
                Connection.Disconnect();                

                ActiveQueries.Clear();                

                Connection = null;
            }, () => IsConnected );            
        }

        private void HandleQueryCreated(object sender, Types.QueryCreatedEventArgs e)
        {
            Query query = e.Query;

            query.QueryId = GetNewQueryID();
            var vm = new QueryViewModel(query, ChartDataFactory) { Started = DateTime.Now };
            ActiveQueries.Add(vm);            

            if (query.Selections.Count == 0)
                vm.IsFinished = true;

            SelectedQuery = vm;
            e.Handled = true;

            // send the query
            Connection.SendQuery(query);
        }

        private void CreateNavigation()
        {
            Navigation.Add(new NavigationElement("Network Information", "infocircle", null));

            var inputQueryViewModel = new InputQueryViewModel();
            inputQueryViewModel.QueryCreated += HandleQueryCreated;
            Navigation.Add(new NavigationElement("Type Query", "edit", inputQueryViewModel));

            var constructQueryViewModel = new ConstructQueryViewModel();
            Navigation.Add(new NavigationElement("Query Designer", "layergroup", constructQueryViewModel));

            var activeQueriesNavigationElement = new NavigationElement("Active Queries", "cogs", null);
            // synchronize the elements in active queries with the list of active queries
            activeQueriesNavigationElement.ReplaceItemsCollection(new SynchronizedObservableCollection<QueryViewModel, NavigationElement>(
                ActiveQueries, 
                (qvm) => new NavigationElement("Query " + qvm.QueryId.ToString(), "file", qvm), 
                (ne) => (QueryViewModel)ne.DataContext));
            Navigation.Add(activeQueriesNavigationElement);

            // synchronize the elements in completed queries with the list of completed queries
            var completedQueriesNavigationElement = new NavigationElement("Completed Queries", "checkdouble", null);
            completedQueriesNavigationElement.ReplaceItemsCollection(new SynchronizedObservableCollection<QueryViewModel, NavigationElement>(
                CompletedQueries,
                (qvm) => new NavigationElement("Query " + qvm.QueryId.ToString(), "file", qvm),
                (ne) => (QueryViewModel)ne.DataContext));
            Navigation.Add(completedQueriesNavigationElement);


            Navigation.Add(new NavigationElement("Query Templates", "marker", null));

            Navigation.Add(new NavigationElement("Node Management", "wrench", null));
        }

        private NavigationElement FindNavigationForDataContext(object dataContext)
        {
            var mainSel = Navigation.FirstOrDefault(x => x.DataContext == dataContext);
            if (mainSel != null)
                return mainSel;

            // search one level deeper
            return Navigation.SelectMany(x => x.Items).FirstOrDefault(x => x.DataContext == dataContext);
        }

        private NavigationElement GetNavigationParent(NavigationElement child)
        {
            return Navigation.FirstOrDefault(x => x.Items.Contains(child));
        }

        #endregion

        #region Queries

        private int GetNewQueryID()
        {
            lastQueryID++;
            return lastQueryID;
        }

        #endregion        

        #region Event handling

        void connection_QueryResultReceived(object sender, QueryResultReceivedEventArgs e)
        {
            dispatcher.Invoke(new Action(() =>
            {
                var query = ActiveQueries.FirstOrDefault(q => q.QueryId == e.QueryId);

                if (query != null)
                {
                    query.IsFinished = !query.IsPeriodic;
                    // add a new resultset
                    var rset = new ResultsetViewModel();
                    rset.Received = DateTime.Now;

                    foreach (var qr in e.Results.Rows)
                        rset.Rows.Add(new QueryResultRowViewModel(query.Selections.Select(s => new ValueSelection(s.Sensor, s.SelectionFunction)), qr));

                    query.Results.Add(rset);
                    if (query.SelectedResultIndex == -1)
                        query.SelectedResultIndex = 0;
                    else
                        if (query.SelectedResultIndex == query.ResultCount - 2) // move selection to latest result set
                            query.SelectedResultIndex = query.ResultCount - 1;

                    // refresh command bindings
                    CommandManager.InvalidateRequerySuggested();
                }
            }), null);
        }

        #endregion
    }    
}
