using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public class DeploymentManifest
    {
        public DeploymentSettings DeploymentSettings { get; private set; }

        public DirectoryInfo SourceDirectory { get; private set; }

        public IList<FileInfo> FilesToDeploy { get; set; }

        public DeploymentManifest(DirectoryInfo sourceDirectory, DeploymentSettings deploymentSettings)
        {
            Require.NotNull(sourceDirectory, "sourceDirectory");
            Require.NotNull(deploymentSettings, "deploymentSettings");

            SourceDirectory = sourceDirectory;
            DeploymentSettings = deploymentSettings;
            FilesToDeploy = new List<FileInfo>();
        }
    }
}
