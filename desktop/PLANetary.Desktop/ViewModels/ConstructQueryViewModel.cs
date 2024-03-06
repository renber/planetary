using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using PLANetary.Core.Types;
using PLANetary.Interaction;
using System.Windows.Input;
using PLANetary.Types;

namespace PLANetary.ViewModels
{
    class ConstructQueryViewModel: ViewModelBase
    {
        
        #region Variables
        #endregion

        #region Properties
        
        /// <summary>
        /// Sensors which can be queried
        /// </summary>
        public ObservableCollection<SensorViewModel> AvailableSensors { get; private set; }

        public ObservableCollection<ActuatorViewModel> AvailableActuators { get; private set; }

        /// <summary>
        /// Selection functions which can be used to query sensors
        /// </summary>
        public ObservableCollection<SelectionFunctionViewModel> AvailableSelectionFunctions { get; private set; }

        /// <summary>
        /// The sensor and functions which have been selected by the user
        /// </summary>
        public ObservableCollection<ValueSelectionViewModel> SelectedSensors { get; private set; }

        /// <summary>
        /// The conditions for the query defined by the user
        /// </summary>
        public ObservableCollection<QueryConditionViewModel> QueryConditions { get; private set; }

        /// <summary>
        /// All available operators to be used in conditions
        /// </summary>
        public ObservableCollection<ConditionOperatorViewModel> AvailableOperators { get; private set; }

        public event EventHandler<QueryCreatedEventArgs> QueryCreated;

        #endregion

        #region Commands

        /// <summary>
        /// Add a value selection by using a AddValueSelectionCommandParameters instance as parameter
        /// </summary>
        public ICommand AddValueSelectionCommand { get;}

        /// <summary>
        /// Remove the ValueSelection (instance as parameter) from SelectedSensors
        /// </summary>
        public ICommand RemoveValueSelectionCommand { get; }

        /// <summary>
        /// Add a query condition
        /// </summary>
        public ICommand AddQueryConditionCommand { get; }

        /// <summary>
        /// Remove the QueryCondition which is given as parameter
        /// </summary>
        public ICommand RemoveQueryConditionCommand { get; }

        public ICommand ExecuteQueryCommand { get;  }

        public ICommand ResetCommand { get; }

        #endregion

        #region Constructor
        public ConstructQueryViewModel()
        {
            // get available sensors            
            AvailableSensors = new ObservableCollection<SensorViewModel>();
            foreach (Sensor sensor in Settings.LoadAvailableSensorsFromXml(Settings.configFilename))
            {
                AvailableSensors.Add(new SensorViewModel(sensor));
            }

            // get available actuators
            AvailableActuators = new ObservableCollection<ActuatorViewModel>();
            foreach (Actuator actuator in Settings.LoadAvailableActuatorsFromXml(Settings.configFilename))
            {
                AvailableActuators.Add(new ActuatorViewModel(actuator));
            }

            // get available selection functions
            AvailableSelectionFunctions = new ObservableCollection<SelectionFunctionViewModel>();
            foreach (SelectionFunction selFunc in Enum.GetValues(typeof(SelectionFunction)))
            {
                AvailableSelectionFunctions.Add(new SelectionFunctionViewModel(selFunc));
            }

            SelectedSensors = new ObservableCollection<ValueSelectionViewModel>();

            // get available operators
            AvailableOperators = new ObservableCollection<ConditionOperatorViewModel>();
            foreach (ConditionOperator op in Enum.GetValues(typeof(ConditionOperator)))
            {
                AvailableOperators.Add(new ConditionOperatorViewModel(op));
            }

            // Conditions
            QueryConditions = new ObservableCollection<QueryConditionViewModel>();            

            // commands
            AddValueSelectionCommand = new RelayCommand<AddValueSelectionCommandParameters>(AddValueSelectionCommand_Execute, AddValueSelectionCommand_CanExecute);
            RemoveValueSelectionCommand = new RelayCommand<ValueSelectionViewModel>(RemoveValueSelectionCommand_Execute, RemoveValueSelectionCommand_CanExecute);

            AddQueryConditionCommand = new RelayCommand(() => QueryConditions.Add(new QueryConditionViewModel(new SensorCondition(AvailableSensors[0].Value, ConditionOperator.OP_GREATER, 0))));
            RemoveQueryConditionCommand = new RelayCommand<QueryConditionViewModel>(RemoveQueryConditionCommand_Execute, RemoveQueryConditionCommand_CanExecute);

            ExecuteQueryCommand = new RelayCommand(ExecuteQueryCommand_Execute, ExecuteQueryCommand_CanExecute);
            ResetCommand = new RelayCommand(() =>
            {
                SelectedSensors.Clear();
                QueryConditions.Clear();
            }, ExecuteQueryCommand_CanExecute);
        }
        #endregion

        #region Command implementation

        #region AddValueSelectionCommand

        protected void AddValueSelectionCommand_Execute(AddValueSelectionCommandParameters p)
        {
            // Add the function, but only if it has not already been added
            SelectedSensors.Add(new ValueSelectionViewModel(new ValueSelection(p.Sensor.Value, p.SelFunc.Value)));
        }

        protected bool AddValueSelectionCommand_CanExecute(AddValueSelectionCommandParameters p)
        {            
            if (p != null)
            {
                // can only be executed if the value is not already present
                if (SelectedSensors.Any(v => v.Sensor == p.Sensor?.Value && v.SelectionFunction == p.SelFunc.Value))
                    return false;
                else
                    return p.Sensor != null && p.SelFunc != null;
            }
            else
                return false;
        }

        #endregion

        #region RemoveValueSelectionCommand

        protected void RemoveValueSelectionCommand_Execute(ValueSelectionViewModel v)
        {
            // remove the element
            SelectedSensors.Remove(v);
        }

        protected bool RemoveValueSelectionCommand_CanExecute(ValueSelectionViewModel v)
        {            
            if (v != null)
            {
                // element can only be removed if it exists in the collection
                return SelectedSensors.Contains(v);
            }
            else
                return false;
        }

        #endregion

        #region RemoveQueryConditionCommand

        protected void RemoveQueryConditionCommand_Execute(QueryConditionViewModel c)
        {            
            // remove the element
            QueryConditions.Remove(c);
        }

        protected bool RemoveQueryConditionCommand_CanExecute(QueryConditionViewModel c)
        {
            if (c != null)
            {
                // element can only be removed if it exists in the collection
                return QueryConditions.Contains(c);
            }
            else
                return false;
        }
        #endregion

        #region ExecuteQueryCommand

        protected void ExecuteQueryCommand_Execute()
        {
            // Create RunningQuery from input
            Query query = new Query();            

            foreach (var vSel in SelectedSensors)
                query.Selections.Add(new ValueSelection(vSel.Sensor, vSel.SelectionFunction));

            query.Conditions.ConditionLink = BooleanLink.AND;
            ConditionGroup cg = new ConditionGroup();
            cg.ConditionLink = BooleanLink.AND;
            foreach (var c in QueryConditions)
            {
                cg.Conditions.Add(new SensorCondition(c.Sensor.Value, c.Operator.Value, c.Value));
            }
            query.Conditions.Conditions.Add(cg);

            if (OnQueryCreated(query))
                ResetCommand.Execute(null);
        }

        protected bool ExecuteQueryCommand_CanExecute()
        {
            return SelectedSensors.Count > 0 && (SelectedSensors.All(s => s.SelectionFunction == SelectionFunction.Single) || SelectedSensors.All(s => s.SelectionFunction != SelectionFunction.Single));
        }

        #endregion        

        #endregion

        protected bool OnQueryCreated(Query query)
        {
            QueryCreatedEventArgs args = new QueryCreatedEventArgs(query);
            QueryCreated?.Invoke(this, args);
            return args.Handled;            
        }
    }

    #region Command parameters

    class AddValueSelectionCommandParameters
    {
        public SensorViewModel Sensor;
        public SelectionFunctionViewModel SelFunc;
    }

    #endregion
}
