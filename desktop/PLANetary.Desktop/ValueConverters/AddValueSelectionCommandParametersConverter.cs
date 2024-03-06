using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using PLANetary.ViewModels;

namespace PLANetary.ValueConverters
{
    public class AddValueSelectionCommandParametersConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            AddValueSelectionCommandParameters parameters = new AddValueSelectionCommandParameters();
            foreach (var obj in values)
            {
                if (obj is SensorViewModel)
                    parameters.Sensor = (SensorViewModel)obj;
                else
                    if (obj is SelectionFunctionViewModel)
                        parameters.SelFunc = (SelectionFunctionViewModel)obj;
            }
            return parameters;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
