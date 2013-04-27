using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XDeploy.Storage;

namespace XDeploy
{
    public class DefaultBackupFolderNameGenerator : IBackupFolderNameGenerator
    {
        public Func<DateTime> Today = () => DateTime.Today;

        public string Generate(IStorageLocation storageLocation)
        {
            var baseFolderName = Today().ToString("yyyy-MM-dd");
            var folderName = baseFolderName;
            var number = 1;

            while(storageLocation.DirectoryExists(folderName))
            {
                folderName = baseFolderName + "-" + number.ToString("00");
                number++;
            }

            return folderName;
        }
    }
}
