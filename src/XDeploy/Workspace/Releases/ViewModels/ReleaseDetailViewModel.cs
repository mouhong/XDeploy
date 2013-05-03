using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDeploy.Storage;
using XDeploy.Workspace.Shell.ViewModels;

namespace XDeploy.Workspace.Releases.ViewModels
{
    public class ReleaseDetailViewModel : Screen
    {
        public ShellViewModel Shell
        {
            get
            {
                return Host.Shell;
            }
        }

        public ProjectReleasesViewModel Host { get; private set; }

        private int _releaseId;

        public int ReleaseId
        {
            get
            {
                return _releaseId;
            }
            set
            {
                if (_releaseId != value)
                {
                    _releaseId = value;
                    NotifyOfPropertyChange(() => ReleaseId);
                }
            }
        }

        private string _releaseName;

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
                }
            }
        }

        public BindableCollection<TargetDeploymentInfoViewModel> TargetDeploymentInfos { get; private set; }

        public ReleaseDetailViewModel(ProjectReleasesViewModel host)
        {
            Host = host;
            TargetDeploymentInfos = new BindableCollection<TargetDeploymentInfoViewModel>();
        }

        public void UpdateFrom(Release release, IEnumerable<DeploymentTarget> allTargets)
        {
            ReleaseId = release.Id;
            ReleaseName = release.Name;

            var viewModels = new List<TargetDeploymentInfoViewModel>();

            foreach (var target in allTargets)
            {
                var targetDeploymentInfo = new TargetDeploymentInfoViewModel
                {
                    TargetId = target.Id,
                    TargetName = target.Name,
                    HasSetBackupLocation = target.BackupLocation != null && !target.BackupLocation.IsEmpty()
                };

                var releaseDeploymentInfo = release.DeploymentInfos.FirstOrDefault(x => x.TargetId == target.Id);
                if (releaseDeploymentInfo != null)
                {
                    targetDeploymentInfo.IsDeployed = true;
                    targetDeploymentInfo.DeployedAt = releaseDeploymentInfo.DeployedAtUtc.ToLocalTime();
                }

                viewModels.Add(targetDeploymentInfo);
            }

            TargetDeploymentInfos = new BindableCollection<TargetDeploymentInfoViewModel>(viewModels);
            NotifyOfPropertyChange(() => TargetDeploymentInfos);
        }

        public IEnumerable<IResult> Backup(TargetDeploymentInfoViewModel item)
        {
            if (Shell.MessageBox.Confirm("Are you sure to make this backup?", null) != System.Windows.MessageBoxResult.Yes)
            {
                yield break;
            }

            Shell.Busy.Processing();

            yield return new AsyncActionResult(context =>
            {
                DeploymentTarget target = null;

                using (var session = Shell.WorkContext.OpenSession())
                {
                    target = session.Get<DeploymentTarget>(item.TargetId);
                }

                var deployDirectory = target.DeployLocation.GetDirectory();
                var backupDirectory = target.BackupLocation.GetDirectory();

                var backuper = new ReleaseBackuper();
                backuper.Backup(
                    Paths.Release(Shell.WorkContext.Project.ProjectDirectory, ReleaseName),
                    deployDirectory,
                    backupDirectory);
            });

            Shell.Busy.Hide();

            Shell.MessageBox.Success("Backup succeeded!", "Success");
        }

        public IEnumerable<IResult> Deploy(TargetDeploymentInfoViewModel item)
        {
            if (Shell.MessageBox.Confirm("Are you sure to deploy the release to this target?", null) != System.Windows.MessageBoxResult.Yes)
            {
                yield break;
            }

            Shell.Busy.Show("Deployment started. This might take several minutes. Please wait...");

            yield return new AsyncActionResult(context =>
            {
                var deployer = new ReleaseDeployer(Shell.WorkContext);
                deployer.Deploy(ReleaseId, item.TargetId);
            });

            Shell.Busy.Hide();

            Shell.MessageBox.Success("Deployment succeeded!", "Success");

            item.IsDeployed = true;
            item.DeployedAt = DateTime.UtcNow;
        }
    }
}
