using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace XDeploy.ViewModels
{
    public class DeploymentTargetListViewModel : Screen
    {
        public ShellViewModel Shell
        {
            get
            {
                return Parent.Shell;
            }
        }

        public ProjectDeploymentTargetsViewModel Parent { get; private set; }

        public ObservableCollection<DeploymentTargetListItemViewModel> Targets { get; private set; }

        public DeploymentTargetListViewModel(ProjectDeploymentTargetsViewModel parent)
        {
            Parent = parent;
            Targets = new ObservableCollection<DeploymentTargetListItemViewModel>();
        }

        public void CreateNewTarget()
        {
            Parent.CreateDeploymentTarget();
        }

        public IEnumerable<IResult> EditTarget(DeploymentTargetListItemViewModel item)
        {
            return Parent.EditDeploymentTarget(item.Id);
        }

        public void UpdateTargets(IEnumerable<DeploymentTargetListItemViewModel> targets)
        {
            Targets = new ObservableCollection<DeploymentTargetListItemViewModel>(targets);
            NotifyOfPropertyChange(() => Targets);
        }
    }
}
