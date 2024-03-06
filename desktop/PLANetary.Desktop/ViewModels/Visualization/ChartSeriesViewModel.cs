using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace PLANetary.ViewModels
{
    class ChartSeriesViewModel : ViewModelBase
    {

        string _title = "Unnamed";
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged("Title");
                }
            }
        }

        public ObservableCollection<Tuple<object, object>> ChartValues { get; private set; }

        public ChartSeriesViewModel()
        {
            ChartValues = new ObservableCollection<Tuple<object, object>>();
        }
    }
}
