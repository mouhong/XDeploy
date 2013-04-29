using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.ViewModels
{
    public class ProjectSummaryViewModel : Screen
    {
        public DeploymentProjectViewModel Project { get; private set; }

        public ProjectSummaryViewModel(DeploymentProjectViewModel project)
        {
            DisplayName = "Summary";
            Project = project;
        }
    }
}
