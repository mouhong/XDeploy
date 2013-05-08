using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using System.Windows;
using XDeploy.Workspace.Shell;

namespace XDeploy.Workspace.DeploymentTargets.Screens
{
    [Export(typeof(DeploymentTargetListViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DeploymentTargetListViewModel : Screen, ITabContentScreen
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

        public bool HasTargets
        {
            get
            {
                return Targets.Count > 0;
            }
        }

        private bool _isLoaded;

        public bool IsLoaded
        {
            get
            {
                return _isLoaded;
            }
            set
            {
                if (_isLoaded != value)
                {
                    _isLoaded = value;
                    NotifyOfPropertyChange(() => IsLoaded);
                }
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
                    NotifyOfPropertyChange(() => HasTargets);
                    IsLoaded = true;
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

        protected override void OnActivate()
        {
            Coroutine.BeginExecute(LoadTargets().GetEnumerator());
            base.OnActivate();
        }
    }
}
