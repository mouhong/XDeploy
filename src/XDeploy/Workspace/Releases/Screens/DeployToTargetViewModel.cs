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
using XDeploy.Storage;
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
            LoadFiles();
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
                        VirtualPath = VirtualPathUtil.GetVirtualPath(file, directory)
                    };
                    Files.Add(fileViewModel);
                }

                Shell.Busy.Hide();
            });
        }

        public IEnumerable<IResult> StartDeployment()
        {
            Progress.HasErrors = false;
            Progress.MaxValue = (Files.Count * 2);
            Progress.IsVisible = true;

            DeploymentTarget target = null;

            yield return new AsyncActionResult(context =>
            {
                target = LoadDeploymentTarget();
            });

            yield return new AsyncActionResult(context =>
            {
                DoBackup(target, Files);
            });

            if (!HasBackupErrors)
            {
                yield return new AsyncActionResult(context =>
                {
                    DoDeploy(target, Files);
                });
            }

            Progress.HasErrors = HasErrors;
        }

        public IEnumerable<IResult> Retry()
        {
            DeploymentTarget target = null;

            yield return new AsyncActionResult(context =>
            {
                target = LoadDeploymentTarget();
            });

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
                    DoBackup(target, files);
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
                    DoDeploy(target, files);
                });
            }

            Progress.HasErrors = HasErrors;
        }

        private void DoBackup(DeploymentTarget target, IEnumerable<FileViewModel> files)
        {
            var hasErrors = false;

            if (target.BackupLocation != null && !target.BackupLocation.IsEmpty())
            {
                var workContext = _workContextAccessor.GetCurrentWorkContext();

                var sourceDirectory = Directories.GetDirectory(Paths.ReleaseFiles(workContext.ProjectDirectory, ReleaseDetail.ReleaseName));
                var backupDirectory = target.BackupLocation.GetDirectory();

                foreach (var file in files)
                {
                    var sourceFile = sourceDirectory.GetFile(file.VirtualPath);
                    var backupFile = backupDirectory.GetFile(file.VirtualPath);

                    file.BackupStatus = ProcessingStatus.InProgress;

                    try
                    {
                        backupFile.OverwriteWith(sourceFile);

                        System.Threading.Thread.Sleep(500);

                        if (file.VirtualPath.Contains("Default.aspx"))
                            throw new IOException("Cannot open file: " + file.VirtualPath);

                        file.BackupStatus = ProcessingStatus.Succeeded;
                    }
                    catch (Exception ex)
                    {
                        file.BackupStatus = ProcessingStatus.Failed;
                        file.BackupErrorMessage = ex.Message;
                        file.BackupErrorDetail = ex.StackTrace;
                        hasErrors = true;
                    }

                    Progress.Value++;
                }
            }

            HasBackupErrors = hasErrors;
        }

        private void DoDeploy(DeploymentTarget target, IEnumerable<FileViewModel> files)
        {
            var hasErrors = false;

            var workContext = _workContextAccessor.GetCurrentWorkContext();

            var sourceDirectory = Directories.GetDirectory(Paths.ReleaseFiles(workContext.ProjectDirectory, ReleaseDetail.ReleaseName));
            var deployDirectory = target.DeployLocation.GetDirectory();

            foreach (var file in files)
            {
                var sourceFile = sourceDirectory.GetFile(file.VirtualPath);
                var targetFile = deployDirectory.GetFile(file.VirtualPath);

                file.DeployStatus = ProcessingStatus.InProgress;

                try
                {
                    targetFile.OverwriteWith(sourceFile);
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
        }

        private DeploymentTarget LoadDeploymentTarget()
        {
            var workContext = _workContextAccessor.GetCurrentWorkContext();

            using (var session = workContext.OpenSession())
            {
                return session.Get<DeploymentTarget>(DeploymentTarget.TargetId);
            }
        }
    }
}
