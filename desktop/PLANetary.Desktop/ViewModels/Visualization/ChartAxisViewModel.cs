using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLANetary.ViewModels
{
    class ChartAxisViewModel : ViewModelBase
    {
        string title;
        public String Title => Model?.Name ?? title;

        public ValueSelectionViewModel Model { get; private set; }

        public ChartAxisViewModel(String title)
        {
            this.title = title ?? "";
        }

        public ChartAxisViewModel(ValueSelectionViewModel selection)
        {
            Model = selection;
        }

        public override string ToString()
        {
            return Title;
        }

    }
}
