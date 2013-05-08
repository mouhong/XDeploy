using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDeploy.Storage;

namespace XDeploy
{
    public static class FileOverwritter
    {
        public static readonly string TempFileExtension = ".xdtemp";

        /// <summary>
        /// Overwrite the content of 'oldFile' with that of 'newFile'.
        /// </summary>
        public static void Overwrite(IFile oldFile, IFile newFile)
        {
            var tempFile = CreateTempFile(oldFile, newFile);
            oldFile.Delete();
            tempFile.Rename(oldFile.Name);
        }

        static IFile CreateTempFile(IFile oldFile, IFile newFile)
        {
            var directory = oldFile.GetDirectory();
            var tempFile = directory.GetFile(oldFile.Name + TempFileExtension);

            if (tempFile.Exists)
            {
                tempFile.Delete();
            }

            tempFile.OverwriteWith(newFile);

            return tempFile;
        }
    }
}
