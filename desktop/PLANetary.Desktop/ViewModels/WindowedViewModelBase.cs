using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLANetary.ViewModels
{
    class WindowedViewModelBase : ViewModelBase
    {
        public event EventHandler OnViewCloseRequested;

        protected void RequestViewClose()
        {
            OnViewCloseRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
