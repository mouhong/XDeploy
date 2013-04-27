using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDeploy.Storage;
using XDeploy.Utils;

namespace XDeploy
{
    public class FileBackuper
    {
        public void Backup(DeploymentManifest manifest, IDirectory deployDirectory, IDirectory backupDirectory)
        {
            Require.NotNull(manifest, "manifest");
            Require.NotNull(deployDirectory, "deployDirectory");
            Require.NotNull(backupDirectory, "backupDirectory");

            foreach (var file in manifest.FilesToDeploy)
            {
                var virtualPath = VirtualPathUtil.GetVirtualPath(file, manifest.SourceDirectory);

                if (!deployDirectory.FileExists(virtualPath)) continue;

                using (var fromStream = deployDirectory.OpenRead(virtualPath))
                using (var toStream = backupDirectory.OpenWrite(virtualPath))
                {
                    var count = 0;
                    var buffer = new byte[2048];

                    while (true)
                    {
                        count = fromStream.Read(buffer, 0, buffer.Length);
                        if (count > 0)
                        {
                            toStream.Write(buffer, 0, count);
                        }
                        else
                        {
                            break;
                        }
                    }

                    toStream.Flush();
                }
            }
        }
    }
}
