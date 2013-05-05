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
using XDeploy.Workspace.Shared.ViewModels;
using XDeploy.Workspace.Shell.ViewModels;

namespace XDeploy.Workspace.Releases.ViewModels
{
    [Export(typeof(DeploymentViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DeploymentViewModel : Screen, IWorkspaceScreen
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

        public ProgressBarViewModel ProgressBar { get; private set; }

        private string _message;

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    NotifyOfPropertyChange(() => Message);
                }
            }
        }

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

        public IWorkspace Workspace
        {
            get
            {
                return this.GetWorkspace();
            }
        }

        public ReleaseDetailViewModel ReleaseDetail { get; private set; }

        public TargetDeploymentInfoViewModel DeploymentTarget { get; private set; }

        private Func<ReleaseListViewModel> _releaseListViewModel;

        [ImportingConstructor]
        public DeploymentViewModel(
            IProjectWorkContextAccessor workContextAccessor,
            Func<ReleaseListViewModel> releaseListViewModel)
        {
            _workContextAccessor = workContextAccessor;
            _releaseListViewModel = releaseListViewModel;
        }

        public void Update(ReleaseDetailViewModel releaseDetail, TargetDeploymentInfoViewModel deploymentTarget)
        {
            ReleaseDetail = releaseDetail;
            DeploymentTarget = deploymentTarget;
            ProgressBar = new ProgressBarViewModel();
            DisplayName = "Deploy Release - " + ReleaseDetail.ReleaseName;
            LoadFiles();
        }

        public void Back()
        {
            var listViewModel = _releaseListViewModel();
            Workspace.ActivateItem(listViewModel);
            listViewModel.LoadAsync(0);
        }

        public void LoadFiles()
        {
            Message = "Loading Release Files...";

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

                Message = "Release Files Are Ready";
            });
        }

        public IEnumerable<IResult> StartDeployment()
        {
            Message = "Deployment Started...";

            ProgressBar.ResetColor();
            ProgressBar.MaxValue = (Files.Count * 2);
            ProgressBar.IsVisible = true;

            DeploymentTarget target = null;

            yield return new AsyncActionResult(context =>
            {
                target = LoadDeploymentTarget();
            });

            yield return new AsyncActionResult(context =>
            {
                Message = "Backupping Files...";
                DoBackup(target, Files);
                Message = "Backup Completed";
            });

            if (!HasBackupErrors)
            {
                yield return new AsyncActionResult(context =>
                {
                    Message = "Deploying Files...";
                    DoDeploy(target, Files);
                    Message = "File Deployment Completed";
                });
            }

            if (HasErrors)
            {
                Message = "Failed";
                ProgressBar.SetError();
            }
            else
            {
                Message = "Success";
            }
        }

        public IEnumerable<IResult> Retry()
        {
            DeploymentTarget target = null;

            yield return new AsyncActionResult(context =>
            {
                target = LoadDeploymentTarget();
            });

            ProgressBar.ResetColor();

            var hasBackupWorkInThisRetry = HasBackupErrors;

            if (HasBackupErrors)
            {
                Message = "Retrying Backup...";

                var files = Files.Where(x => x.BackupStatus == ProcessingStatus.Failed).ToList();

                ProgressBar.MaxValue = files.Count + Files.Count;
                ProgressBar.Value = 0;
                ProgressBar.IsVisible = true;

                yield return new AsyncActionResult(context =>
                {
                    DoBackup(target, files);
                    Message = "Backup Completed";
                });
            }

            if (!HasBackupErrors && HasDeployErrors)
            {
                Message = "Retrying File Deployment...";

                var files = Files.Where(x => x.DeployStatus == ProcessingStatus.Failed).ToList();

                if (!hasBackupWorkInThisRetry)
                {
                    ProgressBar.MaxValue = files.Count;
                    ProgressBar.Value = 0;
                    ProgressBar.IsVisible = true;
                }

                yield return new AsyncActionResult(context =>
                {
                    DoDeploy(target, files);
                    Message = "File Deployment Completed";
                });
            }

            if (HasErrors)
            {
                Message = "Failed";
                ProgressBar.SetError();
            }
            else
            {
                Message = "Success";
            }
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

                    ProgressBar.Value++;
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

                ProgressBar.Value++;
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
