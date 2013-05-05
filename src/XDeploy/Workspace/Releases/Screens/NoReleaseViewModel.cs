using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace XDeploy.Workspace.Releases.Screens
{
    [Export(typeof(NoReleaseViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class NoReleaseViewModel : Screen, ITabContentScreen
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
