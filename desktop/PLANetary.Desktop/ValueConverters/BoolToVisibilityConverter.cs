using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace PLANetary.ValueConverters
{
    class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                if (parameter != null && parameter.ToString().Equals("invert", StringComparison.InvariantCultureIgnoreCase))
                    return (bool)value ? Visibility.Collapsed : Visibility.Visible;
                else
                    return (bool)value ? Visibility.Visible : Visibility.Collapsed;
            }
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
