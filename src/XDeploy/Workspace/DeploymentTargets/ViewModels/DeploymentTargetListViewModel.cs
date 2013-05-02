using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using XDeploy.Workspace.Shell.ViewModels;

namespace XDeploy.Workspace.DeploymentTargets.ViewModels
{
    public class DeploymentTargetListViewModel : Screen
    {
        public ShellViewModel Shell
        {
            get
            {
                return Host.Shell;
            }
        }

        public ProjectDeploymentTargetsViewModel Host { get; private set; }

        public ObservableCollection<DeploymentTargetListItemViewModel> Targets { get; private set; }

        public DeploymentTargetListViewModel(ProjectDeploymentTargetsViewModel host)
        {
            Host = host;
            Targets = new ObservableCollection<DeploymentTargetListItemViewModel>();
        }

        public void CreateNewTarget()
        {
            Host.CreateDeploymentTarget();
        }

        public IEnumerable<IResult> EditTarget(DeploymentTargetListItemViewModel item)
        {
            return Host.EditDeploymentTarget(item.Id);
        }

        public void UpdateTargets(IEnumerable<DeploymentTargetListItemViewModel> targets)
        {
            Targets = new ObservableCollection<DeploymentTargetListItemViewModel>(targets);
            NotifyOfPropertyChange(() => Targets);
        }
    }
}
