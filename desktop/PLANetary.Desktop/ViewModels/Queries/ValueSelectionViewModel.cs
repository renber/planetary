using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLANetary.Core.Types;

namespace PLANetary.ViewModels
{
    class ValueSelectionViewModel: ViewModelBase
    {
        private ValueSelection model;

        public Sensor Sensor
        {
            get
            {
                return model.Sensor;
            }
        }

        public SelectionFunction SelectionFunction
        {
            get
            {
                return model.SelFunction;
            }
        }

        public String Name
        {
            get
            {
                return ColumnName;
            }
        }

        public String ColumnDisplayMember
        {
            get
            {
                return "[" + model.ToString() + "]";
            }
        }

        public string ColumnName
        {
            get
            {
                if (String.IsNullOrEmpty(model.Alias))
                    return model.ToString();
                else
                    return model.Alias;
            }
            set
            {
                if (model.Alias != value)
                {
                    model.Alias = value;
                    OnPropertyChanged("ColumnName");
                }
            }
        }

        public ValueSelectionViewModel(ValueSelection value)
        {
            model = value;
        }
    }
}
