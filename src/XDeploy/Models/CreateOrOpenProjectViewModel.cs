using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Win32;
using XDeploy.Commands;
using System.Windows;

namespace XDeploy.Models
{
    public class CreateOrOpenProjectViewModel : TabContentViewModelBase
    {
        public Workspace Workspace { get; private set; }

        private ICommand _openProjectCommand;

        public ICommand OpenProjectCommand
        {
            get
            {
                if (_openProjectCommand == null)
                {
                    _openProjectCommand = new RelayCommand(OpenProject);
                }

                return _openProjectCommand;
            }
        }

        public CreateOrOpenProjectViewModel(Workspace workspace)
        {
            Workspace = workspace;
        }

        private void OpenProject(object param)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "XDeploy Project|*.xdproj";

            if (dialog.ShowDialog() == true)
            {
                var fileName = dialog.FileName;
                var project = DeploymentProject.LoadFrom(fileName);
                var projectViewModel = new DeploymentProjectViewModel(project);
                Workspace.CurrentProject = projectViewModel;
            }
        }
    }
}
