using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace XDeploy.ViewModels
{
    public class EditDeploymentTargetViewModel : Screen
    {
        public ShellViewModel Shell { get; private set; }

        public DeploymentTargetFormViewModel Form { get; private set; }

        public IDeploymentTargetFormActionAware Host { get; private set; }

        public EditDeploymentTargetViewModel(ShellViewModel shell, IDeploymentTargetFormActionAware host)
        {
            Shell = shell;
            Host = host;
            Form = new DeploymentTargetFormViewModel();
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
