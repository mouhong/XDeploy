using Caliburn.Micro;
using Caliburn.Micro.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using XDeploy.Workspace.Shell.ViewModels;

namespace XDeploy.Workspace.DeploymentTargets.ViewModels
{
    public class EditDeploymentTargetViewModel : ValidatableScreen
    {
        public ShellViewModel Shell { get; private set; }

        public DeploymentTargetFormViewModel Form { get; private set; }

        public IDeploymentTargetFormActionAware Host { get; private set; }

        public bool CanSave
        {
            get
            {
                return !Form.HasErrors;
            }
        }

        public EditDeploymentTargetViewModel(ShellViewModel shell, IDeploymentTargetFormActionAware host)
        {
            Shell = shell;
            Host = host;
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
            Host.OnFormCanceled(Form, FormMode.Edit);
        }

        public IEnumerable<IResult> Save()
        {
            Shell.Busy.Processing();

            yield return new AsyncActionResult(context =>
            {
                using (var session = Shell.WorkContext.OpenSession())
                {
                    var target = session.Get<DeploymentTarget>(Form.Id);
                    Form.UpdateTo(target);
                    session.Commit();
                }
            });

            Host.OnFormSaved(Form, FormMode.Edit);

            Shell.Busy.Hide();
        }
    }
}
