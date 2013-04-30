using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.ViewModels
{
    public class ProjectWorkspaceViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public DeploymentProjectViewModel Project { get; private set; }

        public WorkContext WorkContext { get; private set; }

        public ProjectWorkspaceViewModel(DeploymentProjectViewModel project, WorkContext workContext)
        {
            Project = project;
            WorkContext = workContext;

            ActivateItem(new ProjectSummaryViewModel(project));
            Items.Add(new ProjectReleasesViewModel());
            Items.Add(new ProjectDeployTargetsViewModel(project, workContext));
        }
    }
}
