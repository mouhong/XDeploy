using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace XDeploy.Workspace.Home.ViewModels
{
    [Export(typeof(ProjectSummaryViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ProjectSummaryViewModel : Screen, IWorkspaceScreen
    {
        public DeploymentProjectViewModel Project { get; private set; }

        public ProjectSummaryViewModel()
        {
            DisplayName = "Summary";
        }

        public void Update(DeploymentProjectViewModel project)
        {
            Project = project;
            NotifyOfPropertyChange(() => Project);
        }
    }
}
