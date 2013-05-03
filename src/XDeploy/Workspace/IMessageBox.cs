using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Xceed.Wpf.Toolkit;

namespace XDeploy.Workspace
{
    public interface IMessageBox
    {
        System.Windows.MessageBoxResult Show(string message, string caption, System.Windows.MessageBoxButton button, System.Windows.MessageBoxImage icon);

        System.Windows.MessageBoxResult Confirm(string message, string caption, System.Windows.MessageBoxButton button = System.Windows.MessageBoxButton.YesNo);

        System.Windows.MessageBoxResult Success(string message, string caption = "Success");

        System.Windows.MessageBoxResult Error(string message, string caption = "Error");

        System.Windows.MessageBoxResult Error(Exception exception, string caption = "Error");
    }

    [Export(typeof(IMessageBox))]
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

        public System.Windows.MessageBoxResult Error(Exception exception, string caption)
        {
            return Error(exception.Message + Environment.NewLine + exception.StackTrace, caption);
        }
    }
}
