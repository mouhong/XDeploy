using Caliburn.Micro;
using System;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using XDeploy.Workspace.Shell.ViewModels;
using System.Threading.Tasks;

namespace XDeploy.Workspace.Home.ViewModels
{
    [Export(typeof(WelcomeScreenViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class WelcomeScreenViewModel : Screen, IWorkspaceScreen
    {
        private Func<CreateProjectViewModel> _createProjectViewModel;

        public ShellViewModel Shell
        {
            get
            {
                return this.GetWorkspace().GetShell();
            }
        }

        public HomeWorkspaceViewModel Workspace
        {
            get
            {
                return (HomeWorkspaceViewModel)this.GetWorkspace();
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
