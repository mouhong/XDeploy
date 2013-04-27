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
    public class FileDeployer
    {
        private DirectoryInfo _sourceDirectory;
        private IDirectory _backupDirectory;
        private DeploymentSettings _settings = new DeploymentSettings();

        public DeploymentSettings Settings
        {
            get
            {
                return _settings;
            }
        }

        public IDirectory BackupDirectory
        {
            get
            {
                return _backupDirectory;
            }
            set
            {
                _backupDirectory = value;
            }
        }

        public FileDeployer(string sourceDirectory, DeploymentSettings settings)
        {
            Require.NotNullOrEmpty(sourceDirectory, "sourceDirectory");
            _sourceDirectory = new DirectoryInfo(sourceDirectory);

            if (settings != null)
            {
                _settings = settings;
            }
        }

        public void Deploy(IDirectory deployDirectory)
        {
            Require.NotNull(deployDirectory, "deployDirectory");

            if (_backupDirectory != null)
            {
                if (deployDirectory.FullName.Equals(_backupDirectory.FullName, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Backup directory cannot be same as deploy directory.");
            }
            
            var mainifest = new DeploymentManifestBuilder().BuildManifest(_sourceDirectory, _settings);

            if (_backupDirectory != null && mainifest.FilesToDeploy.Count > 0)
            {
                new FileBackuper().Backup(mainifest, deployDirectory, _backupDirectory);
            }

            Deploy(mainifest, deployDirectory);
        }

        private void Deploy(DeploymentManifest manifest, IDirectory deployDirectory)
        {
            if (manifest.FilesToDeploy.Count == 0)
            {
                return;
            }

            foreach (var file in manifest.FilesToDeploy)
            {
                var fileVirtualPath = VirtualPathUtil.GetVirtualPath(file, manifest.SourceDirectory);
                var directoryVirtualPath = VirtualPathUtil.GetDirectory(fileVirtualPath);

                deployDirectory.EnsureDirectoryCreated(directoryVirtualPath);
                deployDirectory.StoreFile(file, fileVirtualPath, true);
            }
        }
    }
}
