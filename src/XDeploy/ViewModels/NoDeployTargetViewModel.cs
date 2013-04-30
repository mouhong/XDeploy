using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.ViewModels
{
    public class NoDeployTargetViewModel : Screen
    {
        public ProjectDeployTargetsViewModel Parent { get; private set; }

        public NoDeployTargetViewModel(ProjectDeployTargetsViewModel parent)
        {
            Parent = parent;
        }

        public void CreateNewDeployTarget()
        {
            Parent.CreateNewDeployTarget();
        }
    }
}
