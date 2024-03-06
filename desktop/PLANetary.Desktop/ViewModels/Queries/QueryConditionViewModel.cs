using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLANetary.Core.Types;

namespace PLANetary.ViewModels
{
    class QueryConditionViewModel : ViewModelBase
    {
        private SensorCondition model;

        SensorViewModel _sensor = null;
        public SensorViewModel Sensor
        {            
            get
            {                
                if (_sensor == null || _sensor.Value != model.Sensor)
                    _sensor = new SensorViewModel(model.Sensor);

                return _sensor;                
            }
            set
            {
                if (value.Value != model.Sensor)
                {
                    model.Sensor = value.Value;
                    OnPropertyChanged("Sensor");
                }
            }
        }

        ConditionOperatorViewModel _operator = null;
        public ConditionOperatorViewModel Operator
        {
            get
            {
                if (_operator == null || _operator.Value != model.Operator)
                    _operator = new ConditionOperatorViewModel(model.Operator);

                return _operator;
            }
            set
            {
                if (value.Value != model.Operator)
                {
                    model.Operator = value.Value;
                    OnPropertyChanged("Operator");
                }
            }
        }

        public int Value
        {
            get
            {
                return model.Value;
            }
            set
            {
                if (value != model.Value)
                {
                    model.Value = value;
                    OnPropertyChanged("Value");
                }
            }
        }

        public QueryConditionViewModel(SensorCondition condition)
        {
            model = condition;
        }
    }
}
