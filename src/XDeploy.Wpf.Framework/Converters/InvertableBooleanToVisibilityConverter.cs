using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace XDeploy.Wpf.Framework.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class InvertableBooleanToVisibilityConverter : IValueConverter
    {
        enum Parameter { Normal, Inverted }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = (bool)value;
            var direction = parameter == null ? Parameter.Normal : (Parameter)Enum.Parse(typeof(Parameter), (string)parameter);
            if (direction == Parameter.Inverted)
                return b ? Visibility.Collapsed : Visibility.Visible;
            return b ? Visibility.Visible : Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
