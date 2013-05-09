using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace XDeploy.Wpf.Framework.Converters
{
    public class ShortTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var maxchars = parameter == null ? 100 : System.Convert.ToInt32(parameter);
            var shortenString = (value as string).Shorten(maxchars);
            return shortenString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
