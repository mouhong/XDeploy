using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using XDeploy.Validation;
using XDeploy.Workspace.Shell.ViewModels;

namespace XDeploy.Workspace.Releases.ViewModels
{
    public class CreateReleaseViewModel : ValidatableScreen
    {
        public ShellViewModel Shell
        {
            get
            {
                return Host.Shell;
            }
        }

        public IReleaseCreationFormHost Host { get; private set; }

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

        public CreateReleaseViewModel(IReleaseCreationFormHost host, DeploymentProject project)
        {
            Host = host;
            ProjectDirectory = project.ProjectDirectory;
            SourceDirectory = project.SourceDirectory;
            ReleaseDirectory = Paths.Release(ProjectDirectory, String.Empty);
        }

        public void Reset()
        {
            ReleaseName = null;
            ReleaseNotes = null;
        }

        public IEnumerable<IResult> Create()
        {
            var result = Shell.MessageBox.Confirm("Are you sure to create this release?", null, System.Windows.MessageBoxButton.YesNo);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                Shell.Busy.Processing();

                yield return new AsyncActionResult(context =>
                {
                    var creator = new ReleaseCreator(Shell.WorkContext);
                    creator.CreateRelease(ReleaseName, ReleaseNotes);
                });

                Shell.Busy.Hide();
                Shell.MessageBox.Success("Release \"" + ReleaseName + "\" is successfully created.", "Success");
                Host.OnReleaseCreated(this);
            }
        }

        public void Cancel()
        {
            Host.OnReleaseCreationCanceled(this);
        }

        protected void ChangeReleaseName(string name)
        {
            ReleaseName = name.Trim();
        }
    }
}
