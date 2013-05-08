using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using XDeploy.IO;
using XDeploy.Text;
using XDeploy.Utils;
using XDeploy.Workspace.Shared;
using XDeploy.Workspace.Shell;

namespace XDeploy.Workspace.Releases.Screens
{
    [Export(typeof(DeployToTargetViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DeployToTargetViewModel : Screen, ITabContentScreen
    {
        private IProjectWorkContextAccessor _workContextAccessor;

        public ProjectWorkContext WorkContext
        {
            get
            {
                return _workContextAccessor.GetCurrentWorkContext();
            }
        }

        private BindableCollection<FileViewModel> _files;

        public BindableCollection<FileViewModel> Files
        {
            get
            {
                if (_files == null)
                {
                    _files = new BindableCollection<FileViewModel>();
                }
                return _files;
            }
        }

        private string _backupFolderName;

        public string BackupFolderName
        {
            get
            {
                return _backupFolderName;
            }
            set
            {
                if (_backupFolderName != value)
                {
                    _backupFolderName = value;
                    NotifyOfPropertyChange(() => BackupFolderName);
                    NotifyOfPropertyChange(() => BackupLocationUri);
                }
            }
        }

        public string BackupLocationUri
        {
            get
            {
                if (!String.IsNullOrEmpty(DeploymentTarget.BackupLocationUri)
                    && !String.IsNullOrEmpty(BackupFolderName))
                {
                    return Path.Combine(DeploymentTarget.BackupLocationUri, BackupFolderName);
                }

                return null;
            }
        }

        public ProgressViewModel Progress { get; private set; }

        private bool _hasBackupErrors;

        public bool HasBackupErrors
        {
            get
            {
                return _hasBackupErrors;
            }
            set
            {
                if (_hasBackupErrors != value)
                {
                    _hasBackupErrors = value;
                    NotifyOfPropertyChange(() => HasBackupErrors);
                    NotifyOfPropertyChange(() => HasErrors);
                }
            }
        }

        private bool _hasDeployErrors;

        public bool HasDeployErrors
        {
            get
            {
                return _hasDeployErrors;
            }
            set
            {
                if (_hasDeployErrors != value)
                {
                    _hasDeployErrors = value;
                    NotifyOfPropertyChange(() => HasDeployErrors);
                    NotifyOfPropertyChange(() => HasErrors);
                }
            }
        }

        public bool HasErrors
        {
            get
            {
                return HasBackupErrors || HasDeployErrors;
            }
        }

        private bool _isProcessing;

        public bool IsProcessing
        {
            get
            {
                return _isProcessing;
            }
            set
            {
                if (_isProcessing != value)
                {
                    _isProcessing = value;
                    NotifyOfPropertyChange(() => IsProcessing);
                    NotifyOfPropertyChange(() => CanBack);
                    NotifyOfPropertyChange(() => CanStartDeployment);
                    NotifyOfPropertyChange(() => CanRetry);
                }
            }
        }

        public ShellViewModel Shell
        {
            get
            {
                return Workspace.GetShell();
            }
        }

        public ITabWorkspace Workspace
        {
            get
            {
                return this.GetWorkspace();
            }
        }

        public ReleaseDetailViewModel ReleaseDetail { get; private set; }

        public AvailableTargetViewModel DeploymentTarget { get; private set; }

        private Func<ReleaseListViewModel> _releaseListViewModel;

        [ImportingConstructor]
        public DeployToTargetViewModel(
            IProjectWorkContextAccessor workContextAccessor,
            Func<ReleaseListViewModel> releaseListViewModel)
        {
            _workContextAccessor = workContextAccessor;
            _releaseListViewModel = releaseListViewModel;
            Progress = new ProgressViewModel();
        }

        public void Update(ReleaseDetailViewModel releaseDetail, AvailableTargetViewModel deploymentTarget)
        {
            ReleaseDetail = releaseDetail;
            DeploymentTarget = deploymentTarget;
            DisplayName = "Deploy Release " + ReleaseDetail.ReleaseName + " to " + deploymentTarget.TargetName;

            BackupFolderName = Template.Render(deploymentTarget.BackupFolderNameTemplate, new BackupFolderNameTemplateModel
            {
                ReleaseId = releaseDetail.ReleaseId,
                ReleaseName = releaseDetail.ReleaseName
            });
            
            LoadFiles();
        }

        public bool CanBack
        {
            get
            {
                return !IsProcessing;
            }
        }

        public void Back()
        {
            Workspace.ActivateItem(ReleaseDetail);
        }

        public void LoadFiles()
        {
            Shell.Busy.Show("Loading Release Files...");

            Task.Factory.StartNew(() =>
            {
                var workContext = _workContextAccessor.GetCurrentWorkContext();
                var directory = new DirectoryInfo(Paths.ReleaseFiles(workContext.ProjectDirectory, ReleaseDetail.ReleaseName));

                foreach (var file in directory.EnumerateFiles("*", SearchOption.AllDirectories))
                {
                    var fileViewModel = new FileViewModel
                    {
                        VirtualPath = VirtualPathUtil.GetVirtualPath(file, directory),
                        Length = file.Length,
                        IsBackupEnabled = DeploymentTarget.HasSetBackupLocation
                    };
                    Files.Add(fileViewModel);
                }

                Shell.Busy.Hide();
            });
        }

        public bool CanStartDeployment
        {
            get
            {
                return !IsProcessing;
            }
        }

        public IEnumerable<IResult> StartDeployment()
        {
            if (Shell.MessageBox.Confirm("Are you sure to start deployment?", null) != MessageBoxResult.Yes)
            {
                yield break;
            }

            IsProcessing = true;

            Progress.HasErrors = false;
            Progress.MaxValue = DeploymentTarget.HasSetBackupLocation ? (Files.Count * 2) : Files.Count;
            Progress.IsVisible = true;

            var session = WorkContext.OpenSession();

            var target = session.Get<DeploymentTarget>(DeploymentTarget.TargetId);
            var deploymentInfo = new ReleaseDeploymentInfo(target.Id, target.Name);

            yield return new AsyncActionResult(context =>
            {
                DoBackup(target, Files, deploymentInfo);
                if (!HasBackupErrors)
                {
                    DoDeploy(target, Files, deploymentInfo);
                }
            });
            
            if (!HasErrors)
            {
                UpdateDeploymentInfoInDatabase(deploymentInfo);
            }

            Progress.HasErrors = HasErrors;
            IsProcessing = false;
        }

        public bool CanRetry
        {
            get
            {
                return !IsProcessing;
            }
        }

        public IEnumerable<IResult> Retry()
        {
            IsProcessing = true;

            var session = WorkContext.OpenSession();

            var target = session.Get<DeploymentTarget>(DeploymentTarget.TargetId);
            var release = session.Get<Release>(ReleaseDetail.ReleaseId);
            var deploymentInfo = release.FindDeploymentInfo(DeploymentTarget.TargetId) ?? new ReleaseDeploymentInfo(DeploymentTarget.TargetId, DeploymentTarget.TargetName);

            Progress.HasErrors = false;

            var hasBackupWorkInThisRetry = HasBackupErrors;

            if (HasBackupErrors)
            {
                var files = Files.Where(x => x.BackupStatus == ProcessingStatus.Failed).ToList();

                Progress.MaxValue = files.Count + Files.Count;
                Progress.Value = 0;
                Progress.IsVisible = true;

                yield return new AsyncActionResult(context =>
                {
                    DoBackup(target, files, deploymentInfo);
                });
            }

            if (!HasBackupErrors && HasDeployErrors)
            {
                var files = Files.Where(x => x.DeployStatus == ProcessingStatus.Failed).ToList();

                if (!hasBackupWorkInThisRetry)
                {
                    Progress.MaxValue = files.Count;
                    Progress.Value = 0;
                    Progress.IsVisible = true;
                }

                yield return new AsyncActionResult(context =>
                {
                    DoDeploy(target, files, deploymentInfo);
                });
            }

            if (!HasErrors)
            {
                UpdateDeploymentInfoInDatabase(deploymentInfo);
            }

            Progress.HasErrors = HasErrors;
            IsProcessing = false;
        }

        private void DoBackup(DeploymentTarget target, IEnumerable<FileViewModel> files, ReleaseDeploymentInfo deploymentInfo)
        {
            var hasErrors = false;

            if (target.BackupRootLocation != null && !target.BackupRootLocation.IsEmpty())
            {
                var workContext = _workContextAccessor.GetCurrentWorkContext();

                var sourceDirectory = target.DeployLocation.GetDirectory();
                var backupDirectory = target.BackupRootLocation.GetDirectory().GetDirectory(BackupFolderName);

                deploymentInfo.BackupLocationUri = backupDirectory.Uri;

                foreach (var file in files)
                {
                    var sourceFile = sourceDirectory.GetFile(file.VirtualPath);

                    if (sourceFile.Exists)
                    {
                        var backupFile = backupDirectory.GetFile(file.VirtualPath);

                        file.BackupStatus = ProcessingStatus.InProgress;

                        try
                        {
                            FileOverwritter.Overwrite(backupFile, sourceFile);
                            file.BackupStatus = ProcessingStatus.Succeeded;
                        }
                        catch (Exception ex)
                        {
                            file.BackupStatus = ProcessingStatus.Failed;
                            file.BackupErrorMessage = ex.Message;
                            file.BackupErrorDetail = ex.StackTrace;
                            hasErrors = true;
                        }
                    }
                    else
                    {
                        file.BackupStatus = ProcessingStatus.Ignored;
                    }

                    Progress.Value++;
                }
            }

            HasBackupErrors = hasErrors;
        }

        private void DoDeploy(DeploymentTarget target, IEnumerable<FileViewModel> files, ReleaseDeploymentInfo deploymentInfo)
        {
            var hasErrors = false;

            var workContext = _workContextAccessor.GetCurrentWorkContext();

            var sourceDirectory = Directories.GetDirectory(Paths.ReleaseFiles(workContext.ProjectDirectory, ReleaseDetail.ReleaseName));
            var deployDirectory = target.DeployLocation.GetDirectory();

            deploymentInfo.DeployLocationUri = deployDirectory.Uri;

            foreach (var file in files)
            {
                var sourceFile = sourceDirectory.GetFile(file.VirtualPath);
                var targetFile = deployDirectory.GetFile(file.VirtualPath);

                file.DeployStatus = ProcessingStatus.InProgress;

                try
                {
                    FileOverwritter.Overwrite(targetFile, sourceFile);
                    file.DeployStatus = ProcessingStatus.Succeeded;
                }
                catch (Exception ex)
                {
                    file.DeployStatus = ProcessingStatus.Failed;
                    file.DeployErrorMessage = ex.Message;
                    file.DeployErrorDetail = ex.StackTrace;
                    hasErrors = true;
                }

                Progress.Value++;
            }

            HasDeployErrors = hasErrors;

            if (!hasErrors)
            {
                deploymentInfo.DeployedAtUtc = DateTime.UtcNow;
            }
        }

        private void UpdateDeploymentInfoInDatabase(ReleaseDeploymentInfo deploymentInfo)
        {
            using (var session = WorkContext.OpenSession())
            {
                var release = session.Get<Release>(ReleaseDetail.ReleaseId);
                var target = session.Get<DeploymentTarget>(DeploymentTarget.TargetId);
                release.AddDeploymentInfo(deploymentInfo);
                release.LastDeployedAtUtc = deploymentInfo.DeployedAtUtc;
                target.LastDeployedAtUtc = deploymentInfo.DeployedAtUtc;

                if (!String.IsNullOrEmpty(deploymentInfo.BackupLocationUri))
                {
                    target.LastBackuppedAtUtc = deploymentInfo.DeployedAtUtc;
                }

                session.Commit();
            }
        }
    }
}
