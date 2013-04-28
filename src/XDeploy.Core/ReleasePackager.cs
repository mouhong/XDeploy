using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public class ReleasePackager
    {
        private DirectoryInfo _sourceDirectory;
        private DeploymentSettings _settings = new DeploymentSettings();

        public DeploymentSettings Settings
        {
            get
            {
                return _settings;
            }
        }

        public ReleasePackager(string sourceDirectory, DeploymentSettings settings)
        {
            Require.NotNullOrEmpty(sourceDirectory, "sourceDirectory");

            _sourceDirectory = new DirectoryInfo(sourceDirectory);

            if (settings != null)
            {
                _settings = settings;
            }
        }

        public ReleasePackage CreateReleasePackage(string packageName, string releaseNotes, string containingDirectory)
        {
            Require.NotNullOrEmpty(containingDirectory, "containingDirectory");

            var package = new ReleasePackage(packageName, Path.Combine(containingDirectory, packageName));

            var mainifest = new DeploymentManifestBuilder().BuildManifest(_sourceDirectory, _settings);

            if (!Directory.Exists(package.Path))
            {
                Directory.CreateDirectory(package.Path);
            }

            var filesDirectory = new DirectoryInfo(package.FilesDirectoryPath);

            CopyFiles(mainifest, filesDirectory);

            var description = new ReleasePackageManifest
            {
                ReleaseNotes = releaseNotes
            };

            description.Save(package.ManifestFilePath);

            package.Refresh();

            return package;
        }

        private void CopyFiles(DeploymentManifest manifest, DirectoryInfo targetDirectory)
        {
            if (manifest.FilesToDeploy.Count == 0)
            {
                return;
            }

            if (!targetDirectory.Exists)
            {
                targetDirectory.Create();
            }

            foreach (var file in manifest.FilesToDeploy)
            {
                var relativePath = file.FullName.Substring(manifest.SourceDirectory.FullName.Length).TrimStart('\\');
                var targetFilePath = Path.Combine(targetDirectory.FullName, relativePath);
                var targetDirectoryPath = Path.GetDirectoryName(targetFilePath);

                if (!Directory.Exists(targetDirectoryPath))
                {
                    Directory.CreateDirectory(targetDirectoryPath);
                }

                file.CopyTo(targetFilePath, true);
            }
        }
    }

}
