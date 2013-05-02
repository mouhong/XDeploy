using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.Workspace.DeploymentTargets.ViewModels
{
    public class NoDeploymentTargetViewModel : Screen
    {
        public ProjectDeploymentTargetsViewModel Host { get; private set; }

        public NoDeploymentTargetViewModel(ProjectDeploymentTargetsViewModel host)
        {
            Host = host;
        }

        public void CreateNewTarget()
        {
            Host.CreateDeploymentTarget();
        }
    }
}
