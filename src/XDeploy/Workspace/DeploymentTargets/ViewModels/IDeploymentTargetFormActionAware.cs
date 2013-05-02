using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.Workspace.DeploymentTargets.ViewModels
{
    public interface IDeploymentTargetFormActionAware
    {
        void OnFormCanceled(DeploymentTargetFormViewModel viewModel, FormMode mode);

        void OnFormSaved(DeploymentTargetFormViewModel viewModel, FormMode mode);
    }
}
