using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDeploy.ViewModels;

namespace XDeploy.Events
{
    public class CurrentProjectChanged
    {
        public DeploymentProjectViewModel NewProject { get; private set; }

        public CurrentProjectChanged(DeploymentProjectViewModel newProject)
        {
            NewProject = newProject;
        }
    }
}
