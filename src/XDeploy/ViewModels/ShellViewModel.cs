using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using XDeploy.Events;

namespace XDeploy.ViewModels
{
    [Export(typeof(ShellViewModel))]
    public class ShellViewModel : Conductor<IScreen>
    {
        private IScreen _initialView;

        public ShellViewModel()
        {
            DisplayName = "XDeploy";
            _initialView = new WelcomeScreenViewModel(this);
        }

        protected override void OnInitialize()
        {
            ActivateItem(_initialView);
            base.OnInitialize();
        }

        public void OnProjectLoaded(DeploymentProjectViewModel project, WorkContext context)
        {
            ChangeActiveItem(new ProjectWorkspaceViewModel(project, context), true);
        }
    }
}
