using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace PLANetary.ViewModels
{
    class ResultsetViewModel : ViewModelBase
    {
        public ObservableCollection<QueryResultRowViewModel> Rows { get; private set; }

        DateTime _received = DateTime.MaxValue;
        /// <summary>
        /// Date and Time this query has been received
        /// </summary>
        public DateTime Received
        {
            get
            {
                return _received;
            }
            set
            {
                if (_received != value)
                {
                    _received = value;
                    OnPropertyChanged("Received");
                }
            }
        }

        public ResultsetViewModel()
        {
            Rows = new ObservableCollection<QueryResultRowViewModel>();
        }

    }
}
