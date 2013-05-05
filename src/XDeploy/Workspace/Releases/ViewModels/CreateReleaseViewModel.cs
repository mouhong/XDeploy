using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using XDeploy.Workspace.Shell.ViewModels;
using XDeploy.Wpf.Framework.Validation;

namespace XDeploy.Workspace.Releases.ViewModels
{
    [Export(typeof(CreateReleaseViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CreateReleaseViewModel : ValidatableScreen, IWorkspaceScreen
    {
        private IProjectWorkContextAccessor _workContextAccessor;

        public ShellViewModel Shell
        {
            get
            {
                return this.GetWorkspace().GetShell();
            }
        }

        public ReleasesWorkspaceViewModel Workspace
        {
            get
            {
                return (ReleasesWorkspaceViewModel)this.GetWorkspace();
            }
        }
        
        public string ProjectDirectory { get; private set; }

        private string _releaseName;

        [Required]
        public string ReleaseName
        {
            get
            {
                return _releaseName;
            }
            set
            {
                if (_releaseName != value)
                {
                    _releaseName = value;
                    NotifyOfPropertyChange(() => ReleaseName);
                    ReleaseDirectory = Paths.Release(ProjectDirectory, ReleaseName ?? String.Empty);
                    NotifyOfPropertyChange(() => CanCreate);
                }
            }
        }

        public string SourceDirectory { get; private set; }

        private string _releaseDirectory;

        public string ReleaseDirectory
        {
            get
            {
                return _releaseDirectory;
            }
            set
            {
                if (_releaseDirectory != value)
                {
                    _releaseDirectory = value;
                    NotifyOfPropertyChange(() => ReleaseDirectory);
                }
            }
        }

        private string _releaseNotes;

        public string ReleaseNotes
        {
            get
            {
                return _releaseNotes;
            }
            set
            {
                if (_releaseNotes != value)
                {
                    _releaseNotes = value;
                    NotifyOfPropertyChange(() => ReleaseNotes);
                    NotifyOfPropertyChange(() => CanCreate);
                }
            }
        }

        public bool CanCreate
        {
            get
            {
                return !HasErrors;
            }
        }

        [ImportingConstructor]
        public CreateReleaseViewModel(
            IProjectWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;

            var workContext = _workContextAccessor.GetCurrentWorkContext();
            ProjectDirectory = workContext.ProjectDirectory;
            SourceDirectory = workContext.Project.SourceDirectory;
            ReleaseDirectory = Paths.Release(ProjectDirectory, String.Empty);
        }
        
        public IEnumerable<IResult> Create()
        {
            var result = Shell.MessageBox.Confirm("Are you sure to create this release?", null, System.Windows.MessageBoxButton.YesNo);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                Shell.Busy.Processing();

                yield return new AsyncActionResult(context =>
                {
                    var workContext = _workContextAccessor.GetCurrentWorkContext();
                    var creator = new ReleaseCreator(workContext.WorkContext);
                    creator.CreateRelease(ReleaseName, ReleaseNotes);
                });

                Shell.Busy.Hide();
                Shell.MessageBox.Success("Release \"" + ReleaseName + "\" is successfully created.", "Success");

                Workspace.ShowReleaseList();
            }
        }

        public void Cancel()
        {
            Workspace.ShowReleaseList();
        }

        protected void ChangeReleaseName(string name)
        {
            ReleaseName = name.Trim();
        }
    }
}
