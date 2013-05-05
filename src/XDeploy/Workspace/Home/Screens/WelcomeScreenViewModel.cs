using Caliburn.Micro;
using System;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Threading.Tasks;
using XDeploy.Workspace.Shell;

namespace XDeploy.Workspace.Home.Screens
{
    [Export(typeof(WelcomeScreenViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class WelcomeScreenViewModel : Screen, ITabContentScreen
    {
        private Func<CreateProjectViewModel> _createProjectViewModel;

        public ShellViewModel Shell
        {
            get
            {
                return this.GetWorkspace().GetShell();
            }
        }

        public WorkspaceViewModel Workspace
        {
            get
            {
                return (WorkspaceViewModel)this.GetWorkspace();
            }
        }

        [ImportingConstructor]
        public WelcomeScreenViewModel(
            Func<CreateProjectViewModel> createProjectViewModelFactory)
        {
            _createProjectViewModel = createProjectViewModelFactory;
        }

        public void CreateProject()
        {
            var viewModel = _createProjectViewModel();
            this.GetWorkspace().ActivateItem(viewModel);
        }

        public IEnumerable<IResult> OpenProject()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "XDeploy Project|*.xdproj";
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == true)
            {
                return Workspace.OpenProject(dialog.FileName);
            }

            return Enumerable.Empty<IResult>();
        }
    }
}
