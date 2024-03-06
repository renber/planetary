using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLANetary.Core.Types;
using System.Collections.ObjectModel;
using System.Dynamic;

namespace PLANetary.ViewModels
{
    class QueryResultRowViewModel : ViewModelBase
    {

        #region Variables
        
        private ResultRow model;        

        #endregion

        #region Properties

        public ObservableCollection<SensorValueViewModel> Values { get; private set; }

        private List<ValueSelection> selections;

        public String this[String s]
        {
            get
            {
                int idx = selections.FindIndex( (vs) => {return vs.ToString() == s;});
                if (idx == -1 || idx >= Values.Count)
                    return "missing";

                return Values[idx].Value.ToString();
            }
        }

        #endregion

        #region Constructor

        public QueryResultRowViewModel(IEnumerable<ValueSelection> sels, ResultRow resultRow)
        {
            model = resultRow;

            selections = new List<ValueSelection>(sels);

            // add values to observable collection
            Values = new ObservableCollection<SensorValueViewModel>();
            foreach (var v in model.Values)
            {
                Values.Add(new SensorValueViewModel(new SensorValue(v.Value)));
            }
        }

        #endregion

    }
}
