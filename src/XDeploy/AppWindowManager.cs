using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDeploy.Workspace.Shell;

namespace XDeploy
{
    public class AppWindowManager : WindowManager
    {
        protected override System.Windows.Window EnsureWindow(object model, object view, bool isDialog)
        {
            var window = base.EnsureWindow(model, view, isDialog);

            if (model is ShellViewModel)
            {
                window.ResizeMode = System.Windows.ResizeMode.CanMinimize;
            }

            return window;
        }
    }
}
