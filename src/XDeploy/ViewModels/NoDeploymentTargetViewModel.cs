using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.ViewModels
{
    public class NoDeploymentTargetViewModel : Screen
    {
        public ProjectDeploymentTargetsViewModel Parent { get; private set; }

        public NoDeploymentTargetViewModel(ProjectDeploymentTargetsViewModel parent)
        {
            Parent = parent;
        }

        public void CreateNewTarget()
        {
            Parent.CreateDeploymentTarget();
        }
    }
}
