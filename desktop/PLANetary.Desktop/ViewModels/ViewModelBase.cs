using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PLANetary.ViewModels
{
    /// <summary>
    /// Base class of all ViewModels
    /// </summary>
    class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Change the value of the given backing field for the property of the given name
        /// and raise the PropertyChanged event if necessary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="newValue"></param>
        /// <param name="propertyName"></param>
        /// <returns>True if the value was changed (newValue != old value)</returns>
        protected bool ChangeProperty<T>(ref T field, T newValue, [CallerMemberName]String propertyName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                OnPropertyChanged(propertyName);

                return true;
            }

            return false;
        }

        #region INotifyPropertyChanged implementation

        /// <summary>
        /// Fired when a property has changed its value
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raise the PropertyChanged event to inform listener that the given property has changed its value
        /// </summary>
        /// <param name="propertyName">The property which changed its value, or String.Empty to update all databound properties</param>
        protected void OnPropertyChanged([CallerMemberName]String propertyName = "")
        {            
           PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
