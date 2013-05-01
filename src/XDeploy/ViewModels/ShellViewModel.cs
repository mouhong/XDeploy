using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XDeploy.Events;

namespace XDeploy.ViewModels
{
    [Export(typeof(ShellViewModel))]
    public class ShellViewModel : Conductor<IScreen>
    {
        private IScreen _initialView;

        public BusyIndicatorViewModel Busy { get; private set; }

        public ShellViewModel()
        {
            DisplayName = "XDeploy";
            Busy = new BusyIndicatorViewModel();
            _initialView = new WelcomeScreenViewModel(this);
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

                Busy.Show("Loading project...");

                var path = dialog.FileName;

                yield return new AsyncActionResult(context =>
                {
                    var project = new ProjectLoader().LoadFrom(path);
                    workContext = new WorkContext(project);
                    projectViewModel = new DeploymentProjectViewModel(workContext.Project);
                    WorkContext.SetCurrent(workContext);
                })
                .OnError(context => Busy.Hide());

                Busy.Hide();

                yield return new ActionResult(context =>
                {
                    ChangeActiveItem(new ProjectWorkspaceViewModel(this, projectViewModel, workContext), true);
                    NotifyOfPropertyChange(() => CanCloseProject);
                });

                Task.Factory.StartNew(() =>
                {
                    workContext.Database.Initialize();
                });
            }
        }

        public bool CanCloseProject
        {
            get
            {
                return WorkContext.Current != null;
            }
        }

        public void CloseProject()
        {
            var workContext = WorkContext.Current;

            if (workContext != null)
            {
                WorkContext.Current.Dispose();
                WorkContext.SetCurrent(null);

                ChangeActiveItem(new WelcomeScreenViewModel(this), true);
            }
        }

        public void Exit()
        {
            Application.Current.Shutdown();
        }

        protected override void OnInitialize()
        {
            ActivateItem(_initialView);
            base.OnInitialize();
        }
    }
}
