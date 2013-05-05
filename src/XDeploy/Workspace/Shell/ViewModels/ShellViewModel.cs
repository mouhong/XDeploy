using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;

namespace XDeploy.Workspace.Shell.ViewModels
{
    [Export(typeof(ShellViewModel))]
    public class ShellViewModel : Conductor<IWorkspace>.Collection.OneActive
    {
        public IBusyIndicator Busy { get; private set; }

        public IMessageBox MessageBox { get; private set; }

        [ImportingConstructor]
        public ShellViewModel(
            IMessageBox messageBox, 
            IBusyIndicator busy,
            [ImportMany]IEnumerable<IWorkspace> workspaces)
        {
            DisplayName = "XDeploy";
            Busy = busy;
            MessageBox = messageBox;
            Items.AddRange(workspaces);
            Items[0].IsVisible = true;
            ActivateItem(Items[0]);
        }

        public void OnProjectOpened()
        {
            foreach (var item in Items)
            {
                item.IsVisible = true;
            }
        }
    }
}
