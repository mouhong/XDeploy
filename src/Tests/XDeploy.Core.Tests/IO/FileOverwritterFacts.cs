using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XDeploy.IO;
using XDeploy.IO.Local;
using Xunit;

namespace XDeploy.Core.Tests.IO
{
    public class FileOverwritterFacts
    {
        [Fact]
        public void CanOverwriteExistingLocalFile()
        {
            var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
            Directory.CreateDirectory(basePath);

            var oldFileName = Guid.NewGuid().ToString() + ".txt";
            var newFileName = Guid.NewGuid().ToString() + ".txt";

            var oldFilePath = Path.Combine(basePath, oldFileName);
            var newFilePath = Path.Combine(basePath, newFileName);

            File.WriteAllText(oldFilePath, "old file");
            File.WriteAllText(newFilePath, "new");

            try
            {
                FileOverwritter.Overwrite(
                    new LocalFile("/" + oldFileName, new FileInfo(oldFilePath)),
                    new LocalFile("/" + newFileName, new FileInfo(newFilePath))
                    );

                Assert.Equal("new", File.ReadAllText(oldFilePath));
                Assert.Equal("new", File.ReadAllText(oldFilePath));

                // Assert that the temp file should be removed
                Assert.False(File.Exists(oldFilePath + FileOverwritter.TempFileExtension));
            }
            finally
            {
                File.Delete(oldFilePath);
                File.Delete(newFilePath);
            }
        }

        [Fact]
        public void CanCreateNewWhenTheFileToOverwriteDoesNotExist()
        {
            var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
            Directory.CreateDirectory(basePath);

            var oldFileName = Guid.NewGuid().ToString() + ".txt";
            var newFileName = Guid.NewGuid().ToString() + ".txt";

            var oldFilePath = Path.Combine(basePath, oldFileName);
            var newFilePath = Path.Combine(basePath, newFileName);

            File.WriteAllText(newFilePath, "new");

            try
            {
                FileOverwritter.Overwrite(
                    new LocalFile("/" + oldFileName, new FileInfo(oldFilePath)),
                    new LocalFile("/" + newFileName, new FileInfo(newFilePath))
                    );

                Assert.Equal("new", File.ReadAllText(oldFilePath));
                Assert.Equal("new", File.ReadAllText(oldFilePath));

                // Assert that the temp file should be removed
                Assert.False(File.Exists(oldFilePath + FileOverwritter.TempFileExtension));
            }
            finally
            {
                File.Delete(oldFilePath);
                File.Delete(newFilePath);
            }
        }
    }
}
