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

    public class ReleasePackageDeployer
    {
        public event EventHandler<FileDeploymentEventArgs> FileDeploying;

        public event EventHandler<FileDeploymentEventArgs> FileDeployed;

        public void Deploy(ReleasePackage package, IDirectory deployDirectory, IDirectory backupDirectory)
        {
            Require.NotNull(deployDirectory, "deployDirectory");

            if (backupDirectory != null)
            {
                new ReleaseBackuper().Backup(package, deployDirectory, backupDirectory);
            }

            DeployFiles(package, deployDirectory);
        }

        private void DeployFiles(ReleasePackage package, IDirectory deployDirectory)
        {
            var filesDirectory = new DirectoryInfo(package.FilesDirectoryPath);

            if (!filesDirectory.Exists) return;

            foreach (var file in filesDirectory.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                OnFileDeploying(new FileDeploymentEventArgs(file));

                var fileVirtualPath = VirtualPathUtil.GetVirtualPath(file, filesDirectory);
                var directoryVirtualPath = VirtualPathUtil.GetDirectory(fileVirtualPath);

                deployDirectory.EnsureDirectoryCreated(directoryVirtualPath);
                deployDirectory.StoreFile(file, fileVirtualPath, true);

                OnFileDeployed(new FileDeploymentEventArgs(file));
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
