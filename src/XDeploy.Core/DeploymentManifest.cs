using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public class DeploymentManifest
    {
        public DirectoryInfo SourceDirectory { get; private set; }

        public IList<FileInfo> FilesToDeploy { get; set; }

        public DeploymentManifest(DirectoryInfo sourceDirectory)
        {
            Require.NotNull(sourceDirectory, "sourceDirectory");

            SourceDirectory = sourceDirectory;
            FilesToDeploy = new List<FileInfo>();
        }
    }
}
