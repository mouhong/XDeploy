using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XDeploy.Workspace.Home.ViewModels;
using NHibernate.Linq;

namespace XDeploy.Workspace.Shell.ViewModels
{
    [Export(typeof(ShellViewModel))]
    public class ShellViewModel : Conductor<IScreen>
    {
        static NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        public IBusyIndicator Busy { get; private set; }

        public IMessageBox MessageBox { get; private set; }

        public StartupArguments StartupArguments { get; private set; }

        public WorkContext WorkContext { get; private set; }

        private DeploymentProjectViewModel _project;

        public DeploymentProjectViewModel Project
        {
            get
            {
                if (_project == null && WorkContext != null)
                {
                    _project = new DeploymentProjectViewModel(WorkContext.Project);
                }
                return _project;
            }
        }

        [ImportingConstructor]
        public ShellViewModel(
            StartupArguments startupArguments,
            IMessageBox messageBox, 
            IBusyIndicator busy)
        {
            DisplayName = "XDeploy";
            StartupArguments = startupArguments;
            Busy = busy;
            MessageBox = messageBox;
        }

        public void CreateProject()
        {
            ChangeActiveItem(new CreateProjectViewModel(this), true);
        }

        public void SwithchToWelcomeScreen()
        {
            ChangeActiveItem(new WelcomeScreenViewModel(this), true);
        }

        public void SwitchToProjectWorkspaceScreen()
        {
            ChangeActiveItem(new ProjectWorkspaceViewModel(this), true);
            NotifyOfPropertyChange(() => CanCloseProject);
        }

        public IEnumerable<IResult> BrowseProjectAndOpen()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "XDeploy Project|*.xdproj";
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == true)
            {
                return OpenProject(dialog.FileName);
            }

            return Enumerable.Empty<IResult>();
        }

        public IEnumerable<IResult> OpenProject(string path)
        {
            Busy.Loading();

            yield return new AsyncActionResult(context =>
            {
                var project = new ProjectLoader().LoadFrom(path);

                WorkContext = new WorkContext(project);

                Task.Factory.StartNew(() =>
                {
                    ApplicationWarmmer.Warm(WorkContext);
                });

                NotifyOfPropertyChange(() => Project);
            });

            Busy.Hide();
            SwitchToProjectWorkspaceScreen();
        }

        public bool CanCloseProject
        {
            get
            {
                return WorkContext != null;
            }
        }

        public void CloseProject()
        {
            if (WorkContext != null)
            {
                WorkContext.Dispose();
                WorkContext = null;
                SwithchToWelcomeScreen();
            }
        }

        public void Exit()
        {
            Application.Current.Shutdown();
        }

        protected override void OnViewLoaded(object view)
        {
            if (!String.IsNullOrEmpty(StartupArguments.Path))
            {
                Caliburn.Micro.Action.Invoke(this, "OpenProject", (DependencyObject)view, parameters: new object[] { StartupArguments.Path });
            }
            else
            {
                SwithchToWelcomeScreen();
            }
            
            base.OnViewLoaded(view);
        }
    }
}
