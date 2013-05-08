using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Linq;
using System.Windows;
using System.ComponentModel.Composition;
using XDeploy.Workspace.DeploymentTargets.Screens;

namespace XDeploy.Workspace.DeploymentTargets
{
    [Export(typeof(ITabWorkspace))]
    public class WorkspaceViewModel : Conductor<IScreen>.Collection.OneActive, ITabWorkspace
    {
        private Func<DeploymentTargetListViewModel> _deploymentTargetListViewModel;

        public int DisplayOrder
        {
            get
            {
                return 2;
            }
        }

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
        public WorkspaceViewModel(
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

        protected override void OnActivate()
        {
            ShowTargetList();
            base.OnActivate();
        }
    }
}
