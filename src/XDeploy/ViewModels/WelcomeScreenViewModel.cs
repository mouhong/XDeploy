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
        public ShellViewModel Shell { get; private set; }

        public WelcomeScreenViewModel(ShellViewModel shell)
        {
            Shell = shell;
        }

        public IEnumerable<IResult> OpenProject()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "XDeploy Project|*.xdproj";
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == true)
            {
                WorkContext workContext = null;
                DeploymentProjectViewModel projectViewModel = null;

                yield return Loader.Show("Loading project...");

                var path = dialog.FileName;

                yield return new AsyncActionResult(context =>
                {
                    var project = new ProjectLoader().LoadFrom(path);
                    workContext = new WorkContext(project);
                    projectViewModel = DeploymentProjectViewModel.Create(workContext);
                    WorkContext.SetCurrent(workContext);
                })
                .OnSuccess(context => Loader.Hide().Execute(context.ExecutionContext))
                .OnError(context => Loader.Hide().Execute(context.ExecutionContext));

                yield return Loader.Hide();

                yield return new ActionResult(context =>
                {
                    Shell.OnProjectLoaded(projectViewModel, workContext);
                });
            }
        }
    }
}
