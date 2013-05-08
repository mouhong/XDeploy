using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.IO
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

            using (var tempFileStream = tempFile.OpenWrite())
            using (var newFileStream = newFile.OpenRead())
            {
                newFileStream.WriteTo(tempFileStream);
            }

            return tempFile;
        }
    }
}
