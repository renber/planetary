using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLANetary.Core.Types;

namespace PLANetary.ViewModels
{
    class ConditionOperatorViewModel : ViewModelBase
    {
        ConditionOperator _value;
        public ConditionOperator Value
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
                return ToString();
            }
        }

        public ConditionOperatorViewModel(ConditionOperator value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToFriendlyName();
        }

    }
}
