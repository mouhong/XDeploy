using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using XDeploy.Wpf.Framework.Validation;
using System.ComponentModel.Composition;
using XDeploy.Workspace.Shell;

namespace XDeploy.Workspace.DeploymentTargets.Screens
{
    [Export(typeof(CreateDeploymentTargetViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CreateDeploymentTargetViewModel : ValidatableScreen, ITabContentScreen
    {
        private IProjectWorkContextAccessor _workContextAccessor;
        private Func<DeploymentTargetListViewModel> _deploymentTargetListViewModel;

        public ShellViewModel Shell
        {
            get
            {
                return this.GetWorkspace().GetShell();
            }
        }

        public ITabWorkspace Workspace
        {
            get
            {
                return this.GetWorkspace();
            }
        }

        public DeploymentTargetFormViewModel Form { get; private set; }

        public bool CanCreate
        {
            get
            {
                return !Form.HasErrors;
            }
        }

        [ImportingConstructor]
        public CreateDeploymentTargetViewModel(
            IProjectWorkContextAccessor workContextAccessor,
            Func<DeploymentTargetListViewModel> deploymentTargetListViewModelFactory)
        {
            _workContextAccessor = workContextAccessor;
            _deploymentTargetListViewModel = deploymentTargetListViewModelFactory;
            Form = CreateFormViewModel();
        }

        public void Reset()
        {
            Form.Reset();
        }

        private DeploymentTargetFormViewModel CreateFormViewModel()
        {
            var model = new DeploymentTargetFormViewModel();
            model.PropertyChanged += (sender, args) =>
            {
                NotifyOfPropertyChange(() => CanCreate);
            };

            return model;
        }

        public void Cancel()
        {
            var listViewModel = _deploymentTargetListViewModel();
            Workspace.ActivateItem(listViewModel);
        }

        public IEnumerable<IResult> Create()
        {
            Shell.Busy.Processing();

            yield return new AsyncActionResult(context =>
            {
                var workContext = _workContextAccessor.GetCurrentWorkContext();

                using (var session = workContext.OpenSession())
                {
                    var target = new DeploymentTarget();
                    Form.UpdateTo(target);

                    session.Save(target);
                    session.Commit();

                    workContext.WorkContext.Project.TotalDeployTargets = session.Query<DeploymentTarget>().Count();
                    workContext.WorkContext.Project.Save();
                }
            });

            var listViewModel = _deploymentTargetListViewModel();
            Workspace.ActivateItem(listViewModel);

            Shell.Busy.Hide();
        }
    }
}
