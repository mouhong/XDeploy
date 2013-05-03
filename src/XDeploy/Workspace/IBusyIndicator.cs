using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.Workspace
{
    public interface IBusyIndicator
    {
        bool IsBusy { get; set; }

        string Message { get; set; }
    }

    public static class BusyIndicatorExtensions
    {
        public static void Show(this IBusyIndicator busy, string message)
        {
            busy.Message = message;
            busy.IsBusy = true;
        }

        public static void Processing(this IBusyIndicator busy)
        {
            busy.Show("Processing...");
        }

        public static void Loading(this IBusyIndicator busy)
        {
            busy.Show("Loading...");
        }

        public static void Hide(this IBusyIndicator busy)
        {
            busy.IsBusy = false;
        }
    }
}
