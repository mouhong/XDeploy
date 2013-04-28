using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XDeploy.Storage;
using XDeploy.Utils;

namespace XDeploy
{
    public class ReleaseBackuper
    {
        public void Backup(ReleasePackage package, IDirectory deployDirectory, IDirectory backupDirectory)
        {
            var filesDirectory = new DirectoryInfo(package.FilesDirectoryPath);
            TryBackupDirectory(filesDirectory, filesDirectory, deployDirectory, backupDirectory);
        }

        private void TryBackupDirectory(DirectoryInfo directory, DirectoryInfo rootDirectory, IDirectory deployDirectory, IDirectory backupDirectory)
        {
            foreach (var file in directory.GetFiles())
            {
                TryBackupFile(file, rootDirectory, deployDirectory, backupDirectory);
            }

            foreach (var dir in directory.GetDirectories())
            {
                TryBackupDirectory(dir, rootDirectory, deployDirectory, backupDirectory);
            }
        }

        private void TryBackupFile(FileInfo file, DirectoryInfo rootDirectory, IDirectory deployDirectory, IDirectory backupDirectory)
        {
            var virtualPath = VirtualPathUtil.GetVirtualPath(file, rootDirectory);

            if (!deployDirectory.FileExists(virtualPath)) return;

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
