using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace XDeploy.Wpf.Framework.Converters
{
    public class ProcessingStatusToBrushConverter : IValueConverter
    {
        static Dictionary<ProcessingStatus, Brush> _brushes = new Dictionary<ProcessingStatus, Brush>
        {
            { ProcessingStatus.Pending, Brushes.DarkGray },
            { ProcessingStatus.InProgress, new SolidColorBrush(Color.FromRgb(71, 156, 235)) },
            { ProcessingStatus.Failed, new SolidColorBrush(Color.FromRgb(89, 190, 20)) },
            { ProcessingStatus.Succeeded, new SolidColorBrush(Color.FromRgb(248, 82, 48)) }
        };

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return _brushes[(ProcessingStatus)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
