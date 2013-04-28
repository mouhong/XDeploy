using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.Models
{
    public class DeploymentProjectViewModel : ViewModelBase
    {
        public DeploymentProjectViewModel()
        {
        }

        public DeploymentProjectViewModel(DeploymentProject project)
        {
            UpdateFrom(project);
        }

        public void UpdateFrom(DeploymentProject project)
        {
        }
    }
}
