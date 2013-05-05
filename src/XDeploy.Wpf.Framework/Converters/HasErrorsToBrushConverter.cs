using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace XDeploy.Wpf.Framework.Converters
{
    public class HasErrorsToBrushConverter : IValueConverter
    {
        public static readonly Brush NormalBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#59BE14");
        public static readonly Brush ErrorBrush = Brushes.Red;
        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var hasErrors = (bool)value;

            if (hasErrors)
            {
                return ErrorBrush;
            }

            return NormalBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
