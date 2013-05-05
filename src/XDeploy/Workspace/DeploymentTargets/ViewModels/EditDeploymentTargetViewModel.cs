using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using XDeploy.Workspace.Shell.ViewModels;
using XDeploy.Wpf.Framework.Validation;

namespace XDeploy.Workspace.DeploymentTargets.ViewModels
{
    [Export(typeof(EditDeploymentTargetViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class EditDeploymentTargetViewModel : ValidatableScreen, IWorkspaceScreen
    {
        private IProjectWorkContextAccessor _workContextAccessor;

        public ShellViewModel Shell
        {
            get
            {
                return this.GetWorkspace().GetShell();
            }
        }

        public DeploymentTargetsWorkspaceViewModel Workspace
        {
            get
            {
                return (DeploymentTargetsWorkspaceViewModel)this.GetWorkspace();
            }
        }

        public DeploymentTargetFormViewModel Form { get; private set; }

        public bool CanSave
        {
            get
            {
                return !Form.HasErrors;
            }
        }

        [ImportingConstructor]
        public EditDeploymentTargetViewModel(
            IProjectWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
            Form = new DeploymentTargetFormViewModel();
            Form.PropertyChanged += (sender, args) =>
            {
                NotifyOfPropertyChange(() => CanSave);
            };
        }

        public void Update(DeploymentTarget target)
        {
            Form.UpdateFrom(target);
        }

        public void Cancel()
        {
            Workspace.ShowTargetList();
        }

        public IEnumerable<IResult> Save()
        {
            Shell.Busy.Processing();

            yield return new AsyncActionResult(context =>
            {
                var workContext = _workContextAccessor.GetCurrentWorkContext();

                using (var session = workContext.OpenSession())
                {
                    var target = session.Get<DeploymentTarget>(Form.Id);
                    Form.UpdateTo(target);
                    session.Commit();
                }
            });

            Shell.Busy.Hide();

            Workspace.ShowTargetList();
        }
    }
}
