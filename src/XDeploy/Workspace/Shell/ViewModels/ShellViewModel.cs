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
        private IScreen _initialView;

        public BusyIndicatorViewModel Busy { get; private set; }

        public IMessageBox MessageBox { get; private set; }

        public WorkContext WorkContext { get; private set; }

        public DeploymentProjectViewModel Project { get; private set; }

        public ShellViewModel()
        {
            DisplayName = "XDeploy";
            Busy = new BusyIndicatorViewModel();
            MessageBox = new DefaultMessageBox();
            _initialView = new WelcomeScreenViewModel(this);
        }

        public IEnumerable<IResult> OpenProject()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "XDeploy Project|*.xdproj";
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == true)
            {
                Busy.Loading();

                var path = dialog.FileName;

                yield return new AsyncActionResult(context =>
                {
                    var project = new ProjectLoader().LoadFrom(path);
                    WorkContext = new WorkContext(project);
                    Project = new DeploymentProjectViewModel(WorkContext.Project);
                })
                .OnError(context => Busy.Hide());

                Busy.Hide();

                ChangeActiveItem(new ProjectWorkspaceViewModel(this), true);
                NotifyOfPropertyChange(() => CanCloseProject);

                Task.Factory.StartNew(() =>
                {
                    WorkContext.Database.Initialize();
                });
            }
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
