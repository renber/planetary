using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLANetary.Core.Types;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PLANetary.Interaction;
using PLANetary.Communication.Protobuf;
using PLANetary.Types.Statistics;
using PLANetary.Services;

namespace PLANetary.ViewModels
{
    class QueryViewModel : ViewModelBase
    {
        #region Variables
        
        private Core.Types.Query model;

        #endregion

        #region Properties

        public int QueryId
        {
            get
            {
                return model.QueryId;
            }
        }

        public bool IsPeriodic
        {
            get
            {
                return model.Periodic;
            }
        }

        public string QueryType
        {
            get
            {
                if (model.Periodic)
                {
                    return String.Format("period: {0:d} s", model.PeriodInMS / 1000);
                }
                else
                    return "once";
            }
        }

        public string StateStr
        {
            get
            {
                if (!IsFinished)
                  {
                      return "Running";
                  } else
                      return "Finished";
            }
        }

        bool _isFinished = false;
        public bool IsFinished
        {
            get
            {
                return _isFinished;
            }
            set
            {
                if (_isFinished != value)
                {
                    _isFinished = value;
                    OnPropertyChanged("IsFinished");
                    OnPropertyChanged("StateStr");
                }
            }
        }

        public String ExecutionTime
        {
            get
            {
                if (ResultCount == 0)
                    return "";
                else
                    return String.Format("{0:d} ms", (int)((Results[0].Received - startTime)).TotalMilliseconds);
            }
        }

        DateTime startTime;
        public DateTime Started
        {
            get => startTime;
            set
            {
                if (value != startTime)
                {
                    startTime = value;
                    OnPropertyChanged();
                    OnPropertyChanged("ExecutionTime");
                }
            }                        
        }
        
        public DateTime LastResultTime
        {
            get
            {
                if (ResultCount == 0)
                    return DateTime.MaxValue;
                else
                    return Results[ResultCount-1].Received;
            }
        }

        public String LastResultTimeText
        {
            get
            {
                if (LastResultTime == DateTime.MaxValue)
                    return "";
                if (LastResultTime.Date == DateTime.Today)
                {
                    return LastResultTime.ToShortTimeString();
                }
                else
                    return LastResultTime.ToShortDateString() + " " + LastResultTime.ToShortTimeString();
            }
        }

        public String SqlText => model.GetTextRepresentation();        

        public ObservableCollection<ValueSelectionViewModel> Selections { get; private set; }
        public ObservableCollection<ResultsetViewModel> Results { get; private set; }

        public ObservableCollection<ChartAxisViewModel> AvailableAxes { get; } = new ObservableCollection<ChartAxisViewModel>();

        ChartAxisViewModel selectedXAxis;
        public ChartAxisViewModel SelectedXAxis { get => selectedXAxis; set => ChangeProperty(ref selectedXAxis, value); }

        ChartAxisViewModel selectedYAxis;
        public ChartAxisViewModel SelectedYAxis { get => selectedYAxis; set => ChangeProperty(ref selectedYAxis, value); }

        // Axis which represents the period of the resultset
        ChartAxisViewModel PeriodAxis { get; }

        #region Result Table        

        public IChartData ChartData { get; }

        public int ResultCount => Results.Count;

        int selectedResultIndex = -1;
        public int SelectedResultIndex
        {
            get => selectedResultIndex;
            set
            {
                if (selectedResultIndex != value)
                {
                    if (value >= 0 && value < ResultCount)
                    {
                        selectedResultIndex = value;
                    }
                    else
                        selectedResultIndex = ResultCount - 1;

                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SelectedResult));
                }
            }
        }

        public ResultsetViewModel SelectedResult
        {
            get
            {
                if (selectedResultIndex < 0 || selectedResultIndex >= Results.Count)
                    return null;
                else
                    return Results[selectedResultIndex];
            }
            set
            {
                if (SelectedResult != value)
                {
                    SelectedResultIndex = Results.IndexOf(value);
                }
            }
        }

        #endregion

        #endregion

        #region Commands

        public ICommand GotoFirstResultsetCommand { get; private set; }
        public ICommand GotoPreviousResultsetCommand { get; private set; }
        public ICommand GotoNextResultsetCommand { get; private set; }
        public ICommand GotoLastResultsetCommand { get; private set; }

        public ICommand ApplyChartParametersCommand { get; private set; }

        #endregion

        #region Constructor

        public QueryViewModel(Core.Types.Query query, IChartDataFactory chartDataFactory)
        {
            model = query;
            ChartData = chartDataFactory.CreateChartData();

            // Convert ValueSelections to ViewModel
            Selections = new ObservableCollection<ValueSelectionViewModel>();
            query.Selections.ForEach(vs => Selections.Add(new ValueSelectionViewModel(vs)));

            PeriodAxis = new ChartAxisViewModel("Period");
            AvailableAxes.Add(PeriodAxis);
            // Columns to axes
            foreach (var s in Selections)
            {
                AvailableAxes.Add(new ChartAxisViewModel(s));
            }            

            // Convert QueryResultsets to ViewModel
            Results = new ObservableCollection<ResultsetViewModel>();
            Results.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Results_CollectionChanged);
            //query.Results.ForEach(r => Results.Add(new QueryResultRowViewModel(query.Selections, r)));

            // Commands
            GotoFirstResultsetCommand = new RelayCommand(GotoFirstResultsetCommand_Execute, GotoFirstResultsetCommand_CanExecute);
            GotoPreviousResultsetCommand = new RelayCommand(GotoPreviousResultsetCommand_Execute, GotoPreviousResultsetCommand_CanExecute);
            GotoNextResultsetCommand = new RelayCommand(GotoNextResultsetCommand_Execute, GotoNextResultsetCommand_CanExecute);
            GotoLastResultsetCommand = new RelayCommand(GotoLastResultsetCommand_Execute, GotoLastResultsetCommand_CanExecute);

            ApplyChartParametersCommand = new RelayCommand(ApplyChartParametersCommand_Execute, ApplyChartParametersCommand_CanExecute);
        }

        void Results_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // refresh bindings when collection has been changed
            OnPropertyChanged(nameof(ResultCount));
            OnPropertyChanged(nameof(ExecutionTime));
            OnPropertyChanged(nameof(LastResultTimeText));
        }

        #endregion

        #region Command implementations

        #region GotoFirstResultsetCommand

        protected void GotoFirstResultsetCommand_Execute()
        {
            SelectedResultIndex = 0;
        }

        protected bool GotoFirstResultsetCommand_CanExecute()
        {
            return ResultCount > 0 && SelectedResultIndex > 0;
        }

        #endregion

        #region GotoPreviousResultsetCommand

        protected void GotoPreviousResultsetCommand_Execute()
        {
            SelectedResultIndex--;
        }

        protected bool GotoPreviousResultsetCommand_CanExecute()
        {
            return ResultCount > 0 && SelectedResultIndex > 0;
        }

        #endregion

        #region GotoNextResultsetCommand

        protected void GotoNextResultsetCommand_Execute()
        {
            SelectedResultIndex++;
        }

        protected bool GotoNextResultsetCommand_CanExecute()
        {
            return ResultCount > 0 && SelectedResultIndex < ResultCount-1;
        }

        #endregion

        #region GotoLastResultsetCommand

        protected void GotoLastResultsetCommand_Execute()
        {
            SelectedResultIndex = ResultCount - 1;
        }

        protected bool GotoLastResultsetCommand_CanExecute()
        {
            return ResultCount > 0 && SelectedResultIndex < ResultCount - 1;
        }

        #endregion

        #region ApplyChartParametersCommand

        protected void ApplyChartParametersCommand_Execute()
        {
            ChartData.Clear();

            ChartData.AddAxis(AxisOrientation.X, SelectedXAxis.Title);
            ChartData.AddAxis(AxisOrientation.Y, SelectedYAxis.Title);

            int xDataIndex = Selections.IndexOf(SelectedXAxis.Model);
            int yDataIndex = Selections.IndexOf(SelectedYAxis.Model);

            StatSeries<String, float, float> series = new StatSeries<string, float, float>("Data");            

            for (int p = 0; p < ResultCount; p++)
            {
                for(int r = 0; r < Results[p].Rows.Count; r++)
                {
                    float x = SelectedXAxis == PeriodAxis ? (p + 1) : Results[p].Rows[r].Values[xDataIndex].Value;
                    float y = SelectedYAxis == PeriodAxis ? (p + 1) : Results[p].Rows[r].Values[yDataIndex].Value;

                    series.Values.Add(new KeyValuePair<float, float>(x, y));
                }                
            }

            List<StatSeries<String, float, float>> sList = new List<StatSeries<string, float, float>>();
            sList.Add(series);

            ChartData.AddSeries<string, float, float>(SeriesType.Line, sList, (s) => s);

            // Generate the data for charting the given values
          /*  ChartSeries.Clear();

            int selXID = -1;
            if (SelectedXAxis != PeriodXAxis)
                selXID = Selections.IndexOf(SelectedXAxis.Model);

            int selYID = Selections.IndexOf(SelectedYAxis.Model);

            int selSeriesID = -1;
            List<Tuple<float, ChartSeriesViewModel>> series = new List<Tuple<float, ChartSeriesViewModel>>();

            PlottedXAxis = SelectedXAxis.Model.Name;
            PlottedYAxis = SelectedYAxis.Model.Name;

            // Gruppen finden
            if (SelectedSeries != null)
            {
                selSeriesID = Selections.IndexOf(SelectedSeries.Model);

                var groups = Results.SelectMany(x => x.Rows.Select(r => r.Values[selSeriesID].Value)).Distinct();

                foreach (var g in groups)
                {
                    series.Add(new Tuple<float, ChartSeriesViewModel>(g, new ChartSeriesViewModel() { Title = SelectedSeries.Model.Name + " = " + String.Format("{0:0.##}", g) }));
                }
            }
            else
            {
                series.Add(new Tuple<float, ChartSeriesViewModel>(0, new ChartSeriesViewModel() { Title = "" }));
            }

            for (int i = 0; i < ResultCount; i++)
            {
                object xVal;
                float yVal;

                foreach (var group in series)
                {
                    QueryResultRowViewModel row;
                    if (selSeriesID == -1)
                        row = Results[i].Rows.FirstOrDefault();
                    else
                    {
                        row = Results[i].Rows.FirstOrDefault(r => r.Values[selSeriesID].Value == group.Item1);
                    }


                    if (row != null)
                    {
                        if (selXID == -1)
                            xVal = i + 1;
                        else
                            xVal = row.Values[selXID].Value;

                        yVal = row.Values[selYID].Value;

                        group.Item2.ChartValues.Add(new Tuple<object, object>(xVal, yVal));
                    }
                }
            }

            foreach (var g in series)
                ChartSeries.Add(g.Item2);*/
        }

        protected bool ApplyChartParametersCommand_CanExecute()
        {            
            return SelectedXAxis != null && SelectedYAxis != null;
        }

        #endregion

        #endregion
    }
}