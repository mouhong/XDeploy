using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XDeploy.Workspace.Home.Screens;
using XDeploy.Workspace.Shell;

namespace XDeploy.Workspace.Home
{
    [Export(typeof(ITabWorkspace))]
    public class WorkspaceViewModel : Conductor<IScreen>, ITabWorkspace
    {
        private StartupArguments _startupArguments;
        private IProjectWorkContextAccessor _workContextAccessor;
        private Func<WelcomeScreenViewModel> _welcomeScreenViewModel;
        private Func<ProjectSummaryViewModel> _projectSummaryViewModel;

        public ShellViewModel Shell
        {
            get
            {
                return this.GetShell();
            }
        }

        private bool _isVisible;

        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    NotifyOfPropertyChange(() => IsVisible);
                }
            }
        }

        [ImportingConstructor]
        public WorkspaceViewModel(
            StartupArguments startupArguments,
            IProjectWorkContextAccessor workContextAccessor,
            Func<WelcomeScreenViewModel> welcomeScreenViewModel,
            Func<ProjectSummaryViewModel> projectSummaryViewModel)
        {
            DisplayName = "Home";
            _startupArguments = startupArguments;
            _workContextAccessor = workContextAccessor;
            _welcomeScreenViewModel = welcomeScreenViewModel;
            _projectSummaryViewModel = projectSummaryViewModel;
        }

        public IEnumerable<IResult> OpenProject(string path)
        {
            Shell.Busy.Loading();

            yield return new AsyncActionResult(context =>
            {
                var project = new ProjectLoader().LoadFrom(path);
                var workContext = new ProjectWorkContext(project);
                _workContextAccessor.SetCurrentWorkContext(workContext);
                workContext.WarmupAsync();
            });

            Shell.Busy.Hide();

            Shell.OnProjectOpened();

            var summary = _projectSummaryViewModel();
            summary.Update(_workContextAccessor.GetCurrentWorkContext().Project);
            ActivateItem(summary);
        }

        public void ShowWelcome()
        {
            ActivateItem(_welcomeScreenViewModel());
        }

        protected override void OnViewLoaded(object view)
        {
            if (!String.IsNullOrEmpty(_startupArguments.Path))
            {
                Caliburn.Micro.Action.Invoke(this, "OpenProject", (DependencyObject)view, parameters: new object[] { _startupArguments.Path });
            }
            else
            {
                ShowWelcome();
            }

            base.OnViewLoaded(view);
        }
    }
}
