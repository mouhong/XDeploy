using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using XDeploy.IO;
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
                    BackupLocationUri = target.BackupRootLocation == null ? null : target.BackupRootLocation.Uri,
                    BackupFolderNameTemplate = target.BackupFolderNameTemplate,
                    HasSetBackupLocation = target.BackupRootLocation != null && !target.BackupRootLocation.IsEmpty()
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

        public void Deploy(AvailableTargetViewModel item)
        {
            var deploymentViewModel = _deployToTargetViewModel();
            this.GetWorkspace().ActivateItem(deploymentViewModel);
            deploymentViewModel.Update(this, item);
        }
    }
}
