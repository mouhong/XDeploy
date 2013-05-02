using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using XDeploy.Workspace.Shell.ViewModels;

namespace XDeploy.Workspace.DeploymentTargets.ViewModels
{
    public class CreateDeploymentTargetViewModel : Screen
    {
        public ShellViewModel Shell { get; private set; }

        public DeploymentTargetFormViewModel Form { get; private set; }

        public IDeploymentTargetFormActionAware Host { get; private set; }

        public CreateDeploymentTargetViewModel(ShellViewModel shell, IDeploymentTargetFormActionAware host)
        {
            Shell = shell;
            Form = new DeploymentTargetFormViewModel();
            Host = host;
        }

        public void Reset()
        {
            Form = new DeploymentTargetFormViewModel();
            NotifyOfPropertyChange(() => Form);
        }

        public void Cancel()
        {
            Host.OnFormCanceled(Form, FormMode.Add);
        }

        public IEnumerable<IResult> Save()
        {
            Shell.Busy.Processing();

            yield return new AsyncActionResult(context =>
            {
                using (var session = Shell.WorkContext.OpenSession())
                {
                    var target = new DeploymentTarget();
                    Form.UpdateTo(target);

                    session.Save(target);
                    session.Commit();

                    Shell.WorkContext.Project.TotalDeployTargets = session.Query<DeploymentTarget>().Count();
                    Shell.WorkContext.Project.Save();
                }
            });

            Host.OnFormSaved(Form, FormMode.Add);

            Shell.Busy.Hide();
        }
    }
}
