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
        public void Backup(DeploymentManifest manifest, IStorageLocation deployLocation, IStorageLocation backupLocation)
        {
            Require.NotNull(manifest, "manifest");
            Require.NotNull(deployLocation, "deployLocation");
            Require.NotNull(backupLocation, "backupLocation");

            foreach (var file in manifest.FilesToDeploy)
            {
                var virtualPath = VirtualPathUtil.GetVirtualPath(file, manifest.SourceDirectory);

                if (!deployLocation.FileExists(virtualPath)) continue;

                using (var fromStream = deployLocation.OpenRead(virtualPath))
                using (var toStream = backupLocation.OpenWrite(virtualPath))
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
