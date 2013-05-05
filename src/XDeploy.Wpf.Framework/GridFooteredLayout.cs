using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace XDeploy.Wpf.Framework
{
    public static class GridFooteredLayout
    {
        public static DependencyProperty FooterHeightProperty =
            DependencyProperty.RegisterAttached("FooterHeight", typeof(GridLength), typeof(GridFooteredLayout),
                new FrameworkPropertyMetadata(GridLength.Auto, new PropertyChangedCallback(OnFooterHeightChanged)));

        public static GridLength GetFooterHeight(DependencyObject d)
        {
            return (GridLength)d.GetValue(FooterHeightProperty);
        }

        public static void SetFooterHeight(DependencyObject d, object value)
        {
            d.SetValue(FooterHeightProperty, value);
        }

        static void OnFooterHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as Grid;
            var height = GetFooterHeight(d);

            grid.RowDefinitions.Add(new RowDefinition { Height = (GridLength)new GridLengthConverter().ConvertFromString("*") });
            grid.RowDefinitions.Add(new RowDefinition { Height = height });
        }
    }
}
