using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLANetary.Core.Types;

namespace PLANetary.ViewModels
{

    class SelectionFunctionViewModel : ViewModelBase
    {
        SelectionFunction _value;
        public SelectionFunction Value
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

        public SelectionFunctionViewModel(SelectionFunction sensor)
        {
            _value = sensor;
        }

        public override string ToString()
        {
            return Value.ToFriendlyName();
        }

    }
}
