using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLANetary.Core.Types;

namespace PLANetary.ViewModels
{
    class SensorValueViewModel : ViewModelBase
    {

        #region Variables
        
        private SensorValue model;

        #endregion

        #region Properties

        public float Value
        {
            get
            {
                return model.Value;
            }
        }

        #endregion

        #region Constructor

        public SensorValueViewModel(SensorValue value)
        {
            model = value;
        }

        #endregion
    }
}
