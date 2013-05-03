using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using XDeploy.Workspace.Shell.ViewModels;
using XDeploy.Validation;

namespace XDeploy.Workspace.DeploymentTargets.ViewModels
{
    public class CreateDeploymentTargetViewModel : ValidatableScreen
    {
        public ShellViewModel Shell { get; private set; }

        public DeploymentTargetFormViewModel Form { get; private set; }

        public IDeploymentTargetFormActionAware Host { get; private set; }

        public bool CanCreate
        {
            get
            {
                return !Form.HasErrors;
            }
        }

        public CreateDeploymentTargetViewModel(ShellViewModel shell, IDeploymentTargetFormActionAware host)
        {
            Shell = shell;
            Form = CreateFormViewModel();
            Host = host;
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
            Host.OnFormCanceled(Form, FormMode.Add);
        }

        public IEnumerable<IResult> Create()
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
