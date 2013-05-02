using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xceed.Wpf.Toolkit;

namespace XDeploy.Workspace
{
    public interface IMessageBox
    {
        System.Windows.MessageBoxResult Show(string message, string caption, System.Windows.MessageBoxButton button, System.Windows.MessageBoxImage icon);

        System.Windows.MessageBoxResult Confirm(string message, string caption, System.Windows.MessageBoxButton button = System.Windows.MessageBoxButton.YesNoCancel);

        System.Windows.MessageBoxResult Success(string message, string caption);

        System.Windows.MessageBoxResult Error(string message, string caption);
    }

    public class DefaultMessageBox : IMessageBox
    {
        public System.Windows.MessageBoxResult Show(string message, string caption, System.Windows.MessageBoxButton button, System.Windows.MessageBoxImage icon)
        {
            return MessageBox.Show(message, caption, button, icon);
        }

        public System.Windows.MessageBoxResult Confirm(string message, string caption, System.Windows.MessageBoxButton button)
        {
            return Show(message, caption, button, System.Windows.MessageBoxImage.Question);
        }

        public System.Windows.MessageBoxResult Success(string message, string caption)
        {
            return Show(message, caption, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        public System.Windows.MessageBoxResult Error(string message, string caption)
        {
            return Show(message, caption, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }
}
