using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLANetary.Core.Types;

namespace PLANetary.ViewModels
{
    class ActuatorViewModel : ViewModelBase
    {
        Actuator _value;
        public Actuator Value
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

        public ActuatorViewModel(Actuator actuator)
        {
            _value = actuator;
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
                return (obj as ActuatorViewModel).Value == Value;
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
