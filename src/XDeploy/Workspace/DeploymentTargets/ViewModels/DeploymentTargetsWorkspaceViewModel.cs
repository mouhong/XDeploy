using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Linq;
using System.Windows;
using XDeploy.Workspace.Shell.ViewModels;
using System.ComponentModel.Composition;

namespace XDeploy.Workspace.DeploymentTargets.ViewModels
{
    [Export(typeof(IWorkspace))]
    public class DeploymentTargetsWorkspaceViewModel : Conductor<IScreen>.Collection.OneActive, IWorkspace
    {
        private Func<DeploymentTargetListViewModel> _deploymentTargetListViewModel;

        private bool _isVisible;

        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    NotifyOfPropertyChange(() => IsVisible);
                }
            }
        }

        [ImportingConstructor]
        public DeploymentTargetsWorkspaceViewModel(
            IProjectWorkContextAccessor workContextAccessor,
            Func<DeploymentTargetListViewModel> deploymentTargetListViewModelFactory)
        {
            DisplayName = "Deployment Targets";
            _deploymentTargetListViewModel = deploymentTargetListViewModelFactory;
        }

        public void ShowTargetList()
        {
            var listViewModel = _deploymentTargetListViewModel();
            ActivateItem(listViewModel);
        }

        protected override void OnViewLoaded(object view)
        {
            ShowTargetList();
            base.OnViewLoaded(view);
        }
    }
}
