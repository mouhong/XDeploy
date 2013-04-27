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
        private IStorageLocation _backupLocation;
        private DeploymentSettings _settings = new DeploymentSettings();

        public DeploymentSettings Settings
        {
            get
            {
                return _settings;
            }
        }

        public IStorageLocation BackupLocation
        {
            get
            {
                return _backupLocation;
            }
            set
            {
                _backupLocation = value;
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

        public void Deploy(IStorageLocation deployLocation)
        {
            Require.NotNull(deployLocation, "deployLocation");

            if (_backupLocation != null)
            {
                if (deployLocation.FullName.Equals(_backupLocation.FullName, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Backup storage cannot be same as target storage.");
            }

            var mainifest = new DeploymentManifestBuilder().BuildManifest(_sourceDirectory, _settings);

            if (_backupLocation != null && mainifest.FilesToDeploy.Count > 0)
            {
                new FileBackuper().Backup(mainifest, deployLocation, _backupLocation);
            }

            Deploy(mainifest, deployLocation);
        }

        private void Deploy(DeploymentManifest manifest, IStorageLocation targetStorage)
        {
            if (manifest.FilesToDeploy.Count == 0)
            {
                return;
            }

            if (manifest.DeploymentSettings.DeleteExistingFiles)
            {
                targetStorage.ClearDirectory("/");
            }

            foreach (var file in manifest.FilesToDeploy)
            {
                var fileVirtualPath = VirtualPathUtil.GetVirtualPath(file, manifest.SourceDirectory);
                var directoryVirtualPath = VirtualPathUtil.GetDirectory(fileVirtualPath);

                targetStorage.EnsureDirectoryCreated(directoryVirtualPath);
                targetStorage.StoreFile(file, fileVirtualPath, true);
            }
        }
    }
}
