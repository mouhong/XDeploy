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
        public void Backup(string releasePath, IDirectory deployDirectory, IDirectory backupDirectory)
        {
            var filesDirectory = new DirectoryInfo(Path.Combine(releasePath, "Files"));
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
            var fileVirtualPath = VirtualPathUtil.GetVirtualPath(file, rootDirectory);

            var liveFile = deployDirectory.GetFile(fileVirtualPath);

            if (!liveFile.Exists) return;

            var backupFile = backupDirectory.GetFile(fileVirtualPath);
            backupFile.OverwriteWith(liveFile);
        }
    }
}
