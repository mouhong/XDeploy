using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.ViewModels
{
    public class ProjectWorkspaceViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public ShellViewModel Shell { get; private set; }

        public DeploymentProjectViewModel Project { get; private set; }

        public WorkContext WorkContext { get; private set; }

        public ProjectWorkspaceViewModel(ShellViewModel shell, DeploymentProjectViewModel project, WorkContext workContext)
        {
            Shell = shell;
            Project = project;
            WorkContext = workContext;

            ActivateItem(new ProjectSummaryViewModel(project));
            Items.Add(new ProjectReleasesViewModel());
            Items.Add(new ProjectDeployTargetsViewModel(shell, project, workContext));
        }
    }
}
