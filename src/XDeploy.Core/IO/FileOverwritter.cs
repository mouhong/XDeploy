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
        /// Overwrite 'destFile' with 'srcFile'.
        /// </summary>
        public static void Overwrite(IFile destFile, IFile srcFile)
        {
            Require.NotNull(destFile, "destFile");
            Require.NotNull(srcFile, "srcFile");

            var tempFile = CreateTempFile(destFile, srcFile);
            destFile.Delete();
            tempFile.Rename(destFile.Name);
        }

        static IFile CreateTempFile(IFile destFile, IFile srcFile)
        {
            var directory = destFile.GetDirectory();
            var tempFile = directory.GetFile(destFile.Name + TempFileExtension);

            if (tempFile.Exists)
            {
                tempFile.Delete();
            }

            using (var tempFileStream = tempFile.OpenWrite())
            using (var newFileStream = srcFile.OpenRead())
            {
                newFileStream.WriteTo(tempFileStream);
            }

            return tempFile;
        }
    }
}
