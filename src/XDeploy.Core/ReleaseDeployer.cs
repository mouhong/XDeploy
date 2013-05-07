using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using XDeploy.Storage;
using XDeploy.Utils;

namespace XDeploy
{
    public class FileDeploymentEventArgs : EventArgs
    {
        public FileInfo File { get; private set; }

        public FileDeploymentEventArgs(FileInfo file)
        {
            File = file;
        }
    }

    public class ReleaseDeployer
    {
        public event EventHandler<FileDeploymentEventArgs> FileDeploying;

        public event EventHandler<FileDeploymentEventArgs> FileDeployed;

        private WorkContext _workContext;

        public ReleaseDeployer(WorkContext workContext)
        {
            Require.NotNull(workContext, "workContext");
            _workContext = workContext;
        }

        public void Deploy(int releaseId, int targetId)
        {
            var session = _workContext.Database.OpenSession();

            var release = session.Get<Release>(releaseId);
            var target = session.Get<DeploymentTarget>(targetId);

            var log = new DeploymentLog
            {
                ReleaseId = release.Id,
                DeployTargetId = target.Id,
                DeployTargetName = target.Name,
                StartedAtUtc = DateTime.UtcNow
            };

            var deployDirectory = Directories.GetDirectory(
                target.DeployLocation.Uri, target.DeployLocation.UserName, target.DeployLocation.Password);

            IDirectory backupDirectory = null;

            if (target.BackupRootLocation != null)
            {
                backupDirectory = Directories.GetDirectory(
                    target.BackupRootLocation.Uri, target.BackupRootLocation.UserName, target.BackupRootLocation.Password);
            }

            var releasePath = Paths.Release(_workContext.Project.ProjectDirectory, release.Name);

            if (backupDirectory != null)
            {
                log.BackupStartedAtUtc = DateTime.UtcNow;

                try
                {
                    new ReleaseBackuper().Backup(releasePath, deployDirectory, backupDirectory);
                    log.BackupCompletedAtUtc = DateTime.UtcNow;
                    target.LastBackuppedAtUtc = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    log.BackupCompletedAtUtc = DateTime.UtcNow;
                    log.Success = false;
                    log.Message = ex.Message;

                    log.CompletedAtUtc = DateTime.UtcNow;

                    session.Save(log);
                    session.Commit();

                    throw;
                }
            }

            try
            {
                log.DeploymentStartedAtUtc = DateTime.UtcNow;
                DeployFiles(new DirectoryInfo(Path.Combine(releasePath, "Files")), deployDirectory);
                log.DeploymentCompletedAtUtc = DateTime.UtcNow;

                release.LastDeployedAtUtc = DateTime.UtcNow;
                target.LastDeployedAtUtc = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                log.DeploymentCompletedAtUtc = DateTime.UtcNow;
                log.Success = false;
                log.Message = ex.Message;

                log.CompletedAtUtc = DateTime.UtcNow;

                session.Save(log);
                session.Commit();

                throw;
            }

            log.Success = true;
            log.Message = "Deployment succeeded";
            log.CompletedAtUtc = DateTime.UtcNow;

            session.Save(log);
            session.Commit();
        }

        private void DeployFiles(DirectoryInfo sourceDirectory, IDirectory deployDirectory)
        {
            if (!sourceDirectory.Exists) return;

            foreach (var sourceFile in sourceDirectory.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                OnFileDeploying(new FileDeploymentEventArgs(sourceFile));

                var fileVirtualPath = VirtualPathUtil.GetVirtualPath(sourceFile, sourceDirectory);

                var liveFile = deployDirectory.GetFile(fileVirtualPath);

                using (var stream = sourceFile.OpenRead())
                {
                    liveFile.OverwriteWith(stream);
                }

                OnFileDeployed(new FileDeploymentEventArgs(sourceFile));
            }
        }

        private void OnFileDeploying(FileDeploymentEventArgs args)
        {
            if (FileDeploying != null)
                FileDeploying(this, args);
        }

        private void OnFileDeployed(FileDeploymentEventArgs args)
        {
            if (FileDeployed != null)
                FileDeployed(this, args);
        }
    }
}
