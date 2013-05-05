using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace XDeploy.Workspace.DeploymentTargets.Screens
{
    [Export(typeof(NoDeploymentTargetViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class NoDeploymentTargetViewModel : Screen, ITabContentScreen
    {
        private Func<CreateDeploymentTargetViewModel> _createDeploymentTargetViewModel;

        public NoDeploymentTargetViewModel(Func<CreateDeploymentTargetViewModel> createDeploymentTargetViewModel)
        {
            _createDeploymentTargetViewModel = createDeploymentTargetViewModel;
        }

        public void CreateNewTarget()
        {
            this.GetWorkspace().ActivateItem(_createDeploymentTargetViewModel());
        }
    }
}
