using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public class ReleasePackageBuilder
    {
        public void Build(string sourceDirectory, string releasePath, DeploymentSettings settings)
        {
            Require.NotNullOrEmpty(sourceDirectory, "sourceDirectory");
            Require.NotNullOrEmpty(releasePath, "releasePath");

            if (Directory.Exists(releasePath))
            {
                throw new InvalidOperationException("Release directory is not empty.");
            }

            Directory.CreateDirectory(releasePath);

            var mainifest = new DeploymentManifestBuilder().BuildManifest(new DirectoryInfo(sourceDirectory), settings);

            CopyFiles(mainifest, new DirectoryInfo(Path.Combine(releasePath, "Files")));
        }

        private void CopyFiles(DeploymentManifest manifest, DirectoryInfo directory)
        {
            if (manifest.FilesToDeploy.Count == 0)
            {
                return;
            }

            if (!directory.Exists)
            {
                directory.Create();
            }

            foreach (var file in manifest.FilesToDeploy)
            {
                var relativePath = file.FullName.Substring(manifest.SourceDirectory.FullName.Length).TrimStart('\\');
                var targetFilePath = Path.Combine(directory.FullName, relativePath);
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
