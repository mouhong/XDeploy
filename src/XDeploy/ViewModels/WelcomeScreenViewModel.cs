using Caliburn.Micro;
using System;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using XDeploy.Events;

namespace XDeploy.ViewModels
{
    public class WelcomeScreenViewModel : Screen
    {
        private IEventAggregator _events;

        public WelcomeScreenViewModel(IEventAggregator events)
        {
            _events = events;
        }

        public void OpenProject()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "XDeploy Project|*.xdproj";
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == true)
            {
                var path = dialog.FileName;
                var project = DeploymentProject.LoadFrom(path);
                var projectModel = new DeploymentProjectViewModel(project);

                _events.Publish(new CurrentProjectChanged(projectModel));
            }
        }
    }
}
