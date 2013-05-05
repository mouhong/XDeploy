using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using XDeploy.Workspace.Shell.ViewModels;

namespace XDeploy.Workspace.Releases.ViewModels
{
    [Export(typeof(NoReleaseViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class NoReleaseViewModel : Screen, IWorkspaceScreen
    {
        private Func<CreateReleaseViewModel> _createReleaseViewModel;

        [ImportingConstructor]
        public NoReleaseViewModel(
            Func<CreateReleaseViewModel> createReleaseViewModelFactory)
        {
            _createReleaseViewModel = createReleaseViewModelFactory;
        }

        public void CreateNewRelease()
        {
            this.GetWorkspace().ActivateItem(_createReleaseViewModel());
        }
    }
}
