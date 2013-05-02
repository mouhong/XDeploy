using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDeploy.Workspace.DeploymentTargets.ViewModels;
using XDeploy.Workspace.Home.ViewModels;
using XDeploy.Workspace.Releases.ViewModels;

namespace XDeploy.Workspace.Shell.ViewModels
{
    public class ProjectWorkspaceViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public ShellViewModel Shell { get; private set; }

        public WorkContext WorkContext
        {
            get
            {
                return Shell.WorkContext;
            }
        }

        public DeploymentProjectViewModel Project
        {
            get
            {
                return Shell.Project;
            }
        }

        public ProjectWorkspaceViewModel(ShellViewModel shell)
        {
            Require.NotNull(shell, "shell");

            Shell = shell;

            ActivateItem(new ProjectSummaryViewModel(Project));
            Items.Add(new ProjectReleasesViewModel(shell));
            Items.Add(new ProjectDeploymentTargetsViewModel(shell));
        }
    }
}
