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
            { ProcessingStatus.InProgress, (SolidColorBrush)new BrushConverter().ConvertFrom("#127FC0") },
            { ProcessingStatus.Failed, Brushes.Red },
            { ProcessingStatus.Succeeded, (SolidColorBrush)new BrushConverter().ConvertFrom("#4FA613") },
            { ProcessingStatus.Ignored, Brushes.DarkOrange }
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
