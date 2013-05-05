using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using XDeploy.Storage;
using XDeploy.Workspace.Shell;

namespace XDeploy.Workspace.Releases.Screens
{
    [Export(typeof(ReleaseDetailViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReleaseDetailViewModel : Screen, ITabContentScreen
    {
        private IProjectWorkContextAccessor _workContextAccessor;
        private Func<ReleaseListViewModel> _releaseListViewModel;
        private Func<DeployToTargetViewModel> _deployToTargetViewModel;

        public ShellViewModel Shell
        {
            get
            {
                return this.GetWorkspace().GetShell();
            }
        }

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
                    NotifyOfPropertyChange(() => HasReleaseNotes);
                }
            }
        }

        public bool HasReleaseNotes
        {
            get
            {
                return !String.IsNullOrWhiteSpace(ReleaseNotes);
            }
        }

        public BindableCollection<AvailableTargetViewModel> AvailableTargets { get; private set; }

        [ImportingConstructor]
        public ReleaseDetailViewModel(
            IProjectWorkContextAccessor workContextAccessor,
            Func<ReleaseListViewModel> releaseListViewModelFactory,
            Func<DeployToTargetViewModel> deployToTargetViewModelFactory)
        {
            _workContextAccessor = workContextAccessor;
            _releaseListViewModel = releaseListViewModelFactory;
            _deployToTargetViewModel = deployToTargetViewModelFactory;
            AvailableTargets = new BindableCollection<AvailableTargetViewModel>();
        }

        public void UpdateFrom(Release release, IEnumerable<DeploymentTarget> allTargets)
        {
            ReleaseId = release.Id;
            ReleaseName = release.Name;
            ReleaseNotes = release.ReleaseNotes;
            DisplayName = "Release Detail - " + ReleaseName;

            var viewModels = new List<AvailableTargetViewModel>();

            foreach (var target in allTargets)
            {
                var targetDeploymentInfo = new AvailableTargetViewModel
                {
                    TargetId = target.Id,
                    TargetName = target.Name,
                    DeployLocationUri = target.DeployLocation.Uri,
                    BackupLocationUri = target.BackupLocation == null ? null : target.BackupLocation.Uri,
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

            AvailableTargets = new BindableCollection<AvailableTargetViewModel>(viewModels);
            NotifyOfPropertyChange(() => AvailableTargets);
        }

        public void Back()
        {
            var listViewModel = _releaseListViewModel();
            this.GetWorkspace().ActivateItem(listViewModel);
            listViewModel.LoadAsync(0);
        }

        public IEnumerable<IResult> Backup(AvailableTargetViewModel item)
        {
            if (Shell.MessageBox.Confirm("Are you sure to make this backup?", null) != System.Windows.MessageBoxResult.Yes)
            {
                yield break;
            }

            Shell.Busy.Processing();

            yield return new AsyncActionResult(context =>
            {
                DeploymentTarget target = null;

                var workContext = _workContextAccessor.GetCurrentWorkContext();

                using (var session = workContext.OpenSession())
                {
                    target = session.Get<DeploymentTarget>(item.TargetId);
                }

                var deployDirectory = target.DeployLocation.GetDirectory();
                var backupDirectory = target.BackupLocation.GetDirectory();

                var backuper = new ReleaseBackuper();
                backuper.Backup(
                    Paths.Release(workContext.ProjectDirectory, ReleaseName),
                    deployDirectory,
                    backupDirectory);
            });

            Shell.Busy.Hide();

            Shell.MessageBox.Success("Backup succeeded!", "Success");
        }

        public void Deploy(AvailableTargetViewModel item)
        {
            var deploymentViewModel = _deployToTargetViewModel();
            this.GetWorkspace().ActivateItem(deploymentViewModel);
            deploymentViewModel.Update(this, item);
        }
    }
}
