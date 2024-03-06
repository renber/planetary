using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLANetary.Core.Types;

namespace PLANetary.ViewModels
{
    class SensorViewModel : ViewModelBase
    {
        Sensor _value;
        public Sensor Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value != _value)
                {
                    _value = value;
                    OnPropertyChanged("Value");
                    OnPropertyChanged("Name");
                }
            }
        }

        public String Name
        {
            get
            {
                return this.ToString();
            }
        }

        public SensorViewModel(Sensor sensor)
        {
            _value = sensor;
        }

        public override string ToString()
        {
            if (!String.IsNullOrEmpty(Value.FriendlyName))
                return Value.FriendlyName + " (" + Value.Name + ")";
            else
                return Value.Name;
        }

        public override bool Equals(object obj)
        {
            if (obj is SensorViewModel)
            {
                return (obj as SensorViewModel).Value == Value;
            }
            else
                return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }
}
