using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public class DeploymentManifestBuilder
    {
        public DeploymentManifest BuildManifest(DirectoryInfo sourceDirectory, DeploymentSettings settings)
        {
            var manifest = new DeploymentManifest(sourceDirectory, settings)
            {
                FilesToDeploy = CollectDeployableFiles(sourceDirectory, sourceDirectory, settings)
            };

            return manifest;
        }

        private List<FileInfo> CollectDeployableFiles(DirectoryInfo directory, DirectoryInfo rootDirectory, DeploymentSettings settings)
        {
            var files = new List<FileInfo>();

            foreach (var entry in directory.GetFileSystemInfos())
            {
                var ignore = false;

                foreach (var rule in settings.IgnorantRules)
                {
                    if (rule.ShouldIgnore(entry, rootDirectory))
                    {
                        ignore = true;
                        break;
                    }
                }

                if (!ignore)
                {
                    if (entry is FileInfo)
                    {
                        files.Add((FileInfo)entry);
                    }
                    else
                    {
                        files.AddRange(CollectDeployableFiles((DirectoryInfo)entry, rootDirectory, settings));
                    }
                }
            }

            return files;
        }
    }
}
