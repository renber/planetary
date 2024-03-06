using PLANetary.Communication.Connection;
using PLANetary.Core.Connection;
using PLANetary.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PLANetary.ViewModels.Connection
{
    abstract class ConnectionParamsViewModel : ViewModelBase
    {
        public abstract string Title { get; }

        public virtual DataTemplate ControlTemplate { get; protected set; }

        /// <summary>
        /// Return whether a connection attempt can be made using the provided parameters
        /// </summary>
        /// <returns></returns>
        public abstract bool CanConnect();        

        /// <summary>
        /// Create a new instance of the planetary connection which uses the parameters
        /// of this view model
        /// </summary>
        public abstract IPlanetaryConnection CreateConnectionInstance();

        /// <summary>
        /// Retrieve the IPlanetaryConnectionParameters contained in this ViewModel
        /// </summary>
        public abstract IPlanetaryConnectionParameters GetParameters();
    }
}
