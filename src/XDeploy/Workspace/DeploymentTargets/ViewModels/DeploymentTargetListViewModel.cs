using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using XDeploy.Workspace.Shell.ViewModels;
using System.Windows;

namespace XDeploy.Workspace.DeploymentTargets.ViewModels
{
    [Export(typeof(DeploymentTargetListViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DeploymentTargetListViewModel : Screen, IWorkspaceScreen
    {
        private IProjectWorkContextAccessor _workContextAccessor;
        private Func<CreateDeploymentTargetViewModel> _createDeploymentTargetViewModel;
        private Func<EditDeploymentTargetViewModel> _editDeploymentTargetViewModel;

        public ShellViewModel Shell
        {
            get
            {
                return this.GetWorkspace().GetShell();
            }
        }

        public ObservableCollection<DeploymentTargetListItemViewModel> Targets { get; private set; }

        [ImportingConstructor]
        public DeploymentTargetListViewModel(
            IProjectWorkContextAccessor workContextAccessor,
            Func<CreateDeploymentTargetViewModel> createDeploymentTargetViewModelFactory,
            Func<EditDeploymentTargetViewModel> editDeploymentTargetViewModelFactory)
        {
            _workContextAccessor = workContextAccessor;
            _createDeploymentTargetViewModel = createDeploymentTargetViewModelFactory;
            _editDeploymentTargetViewModel = editDeploymentTargetViewModelFactory;
            Targets = new ObservableCollection<DeploymentTargetListItemViewModel>();
        }

        public void CreateNewTarget()
        {
            this.GetWorkspace().ActivateItem(_createDeploymentTargetViewModel());
        }

        public IEnumerable<IResult> LoadTargets()
        {
            Shell.Busy.Loading();

            yield return new AsyncActionResult(context =>
            {
                var workContext = _workContextAccessor.GetCurrentWorkContext();

                using (var session = workContext.OpenSession())
                {
                    var targets = session.Query<DeploymentTarget>()
                                         .OrderBy(x => x.Id)
                                         .ToList();
                    Targets = new ObservableCollection<DeploymentTargetListItemViewModel>(
                        targets.Select(x => new DeploymentTargetListItemViewModel(x)));
                    NotifyOfPropertyChange(() => Targets);
                }
            });

            Shell.Busy.Hide();
        }

        public IEnumerable<IResult> EditTarget(DeploymentTargetListItemViewModel item)
        {
            Shell.Busy.Loading();

            DeploymentTarget target = null;

            yield return new AsyncActionResult(context =>
            {
                var workContext = _workContextAccessor.GetCurrentWorkContext();
                using (var session = workContext.OpenSession())
                {
                    target = session.Get<DeploymentTarget>(item.Id);
                }
            });

            var screen = _editDeploymentTargetViewModel();
            screen.Update(target);

            this.GetWorkspace().ActivateItem(screen);

            Shell.Busy.Hide();
        }

        protected override void OnViewLoaded(object view)
        {
            Caliburn.Micro.Action.Invoke(this, "LoadTargets", (DependencyObject)view);
            base.OnViewLoaded(view);
        }
    }
}
