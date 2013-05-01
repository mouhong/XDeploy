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

        public int TargetId { get; set; }

        public DeploymentTargetFormViewModel Form { get; private set; }

        public IDeploymentTargetFormActionAware Host { get; private set; }

        public EditDeploymentTargetViewModel(ShellViewModel shell, IDeploymentTargetFormActionAware host)
        {
            Shell = shell;
            Host = host;
            Form = new DeploymentTargetFormViewModel();
        }

        public void UpdateFrom(DeploymentTarget target)
        {
            Form.UpdateFrom(target);
        }
    }
}
