using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace XDeploy.Models
{
    public class HomePageViewModel : PageViewModelBase
    {
        public Workspace Workspace { get; private set; }

        public CreateOrOpenProjectViewModel CreateOrOpenProjectViewModel { get; private set; }

        public ProjectSummaryViewModel ProjectSummaryViewModel { get; private set; }

        public HomePageViewModel(Workspace workspace)
        {
            Title = "Home";
            Workspace = workspace;
            CreateOrOpenProjectViewModel = new CreateOrOpenProjectViewModel(Workspace)
            {
                Visibility = Visibility.Visible
            };
            ProjectSummaryViewModel = new ProjectSummaryViewModel
            {
                Visibility = Visibility.Hidden
            };
        }

        public override void OnProjectLoaded(DeploymentProjectViewModel project)
        {
            ProjectSummaryViewModel.CurrentProject = project;

            CreateOrOpenProjectViewModel.Visibility = Visibility.Hidden;
            ProjectSummaryViewModel.Visibility = Visibility.Visible;
        }
    }
}
