using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using XDeploy.Changes;
using XDeploy.IO;
using XDeploy.IO.InMemory;

namespace XDeploy.Core.Tests.Changes
{
    public class FileChangeCollectorFacts
    {
        [Fact]
        public void WillNotCollectIgnoredFilesAndDirectories()
        {
            var sourceDirectory = new InMemoryDirectory("/", "D:\\Source", null);
            sourceDirectory.CreateFile("Default.cs", "Default CS", Encoding.UTF8);
            sourceDirectory.CreateFile("Project.csproj", "Project File", Encoding.UTF8);
            sourceDirectory.CreateFile("robots.txt", "Robots", Encoding.UTF8);

            var obj = sourceDirectory.CreateSubDirectory("obj");
            obj.CreateFile("File1.aspx", "obj File1", Encoding.UTF8);

            var subDirectory = sourceDirectory.CreateSubDirectory("Admin");
            subDirectory.CreateFile("List.aspx", "Admin List", Encoding.UTF8);
            subDirectory.CreateFile("List.aspx.cs", "Admin List CS", Encoding.UTF8);

            var backup = subDirectory.CreateSubDirectory("Backup");
            backup.CreateFile("Backup1.aspx", "Backup Backup1", Encoding.UTF8);

            var collector = new FileChangeCollector();
            var context = new FileChangeCollectionContext(sourceDirectory, null, new List<AbstractIgnorantRule>
            {
                new MockIgnorantRule {
                    IgnoredFileVirtualPaths = new List<string> { "/Default.cs", "/Admin/List.aspx.cs", "/Project.csproj" },
                    IgnoredDirectoryVirtualPaths = new List<string> { "/obj", "/Admin/Backup" }
                }
            });

            var changes = collector.Collect(context);

            Assert.False(changes.Any(x => x.File.VirtualPath == "/Default.cs"));
            Assert.False(changes.Any(x => x.File.VirtualPath == "/Admin/List.aspx.cs"));
            Assert.False(changes.Any(x => x.File.VirtualPath == "/Project.csproj"));
            Assert.False(changes.Any(x => x.File.VirtualPath == "/obj/File1.aspx"));
            Assert.False(changes.Any(x => x.File.VirtualPath == "/Admin/Backup/Backup1.aspx"));
        }

        [Fact]
        public void WillReturnForNewAndChangedFiles()
        {
            var sourceDirectory = new InMemoryDirectory("/", "D:\\Source", null);

            sourceDirectory.CreateFile("File1.txt", "File1 V1", Encoding.UTF8);
            sourceDirectory.CreateFile("File2.sql", "File2 V1", Encoding.UTF8);
            sourceDirectory.CreateFile("File3.cs", "File3 V2", Encoding.UTF8);
            sourceDirectory.CreateFile("File4.cs", "File4 V1", Encoding.UTF8);

            var currentChecksums = new List<FileChecksum>
            {
                new FileChecksum("/File3.cs", ChecksumComputer.Compute("File3 V1", Encoding.UTF8)),
                new FileChecksum("/File4.cs", ChecksumComputer.Compute("File4 V1", Encoding.UTF8))
            };

            var collector = new FileChangeCollector();
            var context = new FileChangeCollectionContext(sourceDirectory, currentChecksums, null);

            var changes = collector.Collect(context);

            var file1Change = changes.FirstOrDefault(x => x.File.VirtualPath == "/File1.txt");
            AssertFileChange(file1Change, true, ChecksumComputer.Compute("File1 V1", Encoding.UTF8));
            Assert.Null(file1Change.OldChecksum);

            var file2Change = changes.FirstOrDefault(x => x.File.VirtualPath == "/File2.sql");
            AssertFileChange(file2Change, true, ChecksumComputer.Compute("File2 V1", Encoding.UTF8));
            Assert.Null(file2Change.OldChecksum);

            var file3Change = changes.FirstOrDefault(x => x.File.VirtualPath == "/File3.cs");
            AssertFileChange(file3Change, false, ChecksumComputer.Compute("File3 V2", Encoding.UTF8));
            Assert.NotNull(file3Change.OldChecksum);
            Assert.Equal(ChecksumComputer.Compute("File3 V1", Encoding.UTF8), file3Change.OldChecksum.Checksum);

            var file4Change = changes.FirstOrDefault(x => x.File.VirtualPath == "/File4.cs");
            Assert.Null(file4Change);
        }

        [Fact]
        public void WillTreatFileVirtualPathCaseInsensitive()
        {
            // File name should be treat case insensitive

            var sourceDirectory = new InMemoryDirectory("/", "D:\\Source", null);
            sourceDirectory.CreateFile("File1.aspx", "File1 V1", Encoding.UTF8);

            var checksums = new List<FileChecksum>
            {
                new FileChecksum("/file1.aspx", ChecksumComputer.Compute("File1 V1", Encoding.UTF8))
            };

            var changes = new FileChangeCollector().Collect(new FileChangeCollectionContext(sourceDirectory, checksums, null));
            Assert.Empty(changes);

            sourceDirectory = new InMemoryDirectory("/", "D:\\Source", null);
            sourceDirectory.CreateFile("File1.aspx", "File1 V2", Encoding.UTF8);

            checksums = new List<FileChecksum>
            {
                new FileChecksum("/file1.aspx", ChecksumComputer.Compute("File1 V1", Encoding.UTF8))
            };

            changes = new FileChangeCollector().Collect(new FileChangeCollectionContext(sourceDirectory, checksums, null));
            Assert.Equal(1, changes.Count());
            AssertFileChange(changes.FirstOrDefault(), false, ChecksumComputer.Compute("File1 V2", Encoding.UTF8));
            Assert.Equal(ChecksumComputer.Compute("File1 V1", Encoding.UTF8), changes.First().OldChecksum.Checksum);

            // Directory name should be treat case insensitive

            sourceDirectory = new InMemoryDirectory("/", "D:\\Source", null);

            var adminDirectory = sourceDirectory.CreateSubDirectory("admin");
            adminDirectory.CreateFile("Default.aspx", "Admin Default V1", Encoding.UTF8);

            checksums = new List<FileChecksum>
            {
                new FileChecksum("/Admin/Default.aspx", ChecksumComputer.Compute("Admin Default V1", Encoding.UTF8))
            };

            changes = new FileChangeCollector().Collect(new FileChangeCollectionContext(sourceDirectory, checksums, null));
            Assert.Empty(changes);

            sourceDirectory = new InMemoryDirectory("/", "D:\\Source", null);

            adminDirectory = sourceDirectory.CreateSubDirectory("admin");
            adminDirectory.CreateFile("Default.aspx", "Admin Default V2", Encoding.UTF8);

            checksums = new List<FileChecksum>
            {
                new FileChecksum("/Admin/Default.aspx", ChecksumComputer.Compute("Admin Default V1", Encoding.UTF8))
            };

            changes = new FileChangeCollector().Collect(new FileChangeCollectionContext(sourceDirectory, checksums, null));

            Assert.Equal(1, changes.Count());
            AssertFileChange(changes.FirstOrDefault(), false, ChecksumComputer.Compute("Admin Default V2", Encoding.UTF8));
            Assert.Equal(ChecksumComputer.Compute("Admin Default V1", Encoding.UTF8), changes.First().OldChecksum.Checksum);
        }

        [Fact]
        public void CanHandleDirectoryHierarchy()
        {
            var sourceDirectory = new InMemoryDirectory("/", "D:\\Source", null);

            sourceDirectory.CreateFile("File1.aspx", "File1 New", Encoding.UTF8);
            sourceDirectory.CreateFile("File2.aspx", "File2 V2", Encoding.UTF8);
            sourceDirectory.CreateFile("File3.aspx", "File3 V1", Encoding.UTF8);

            var adminDirectory = sourceDirectory.CreateSubDirectory("Admin");
            adminDirectory.CreateFile("AdminFile1.aspx", "Admin File1 V1", Encoding.UTF8);
            adminDirectory.CreateFile("AdminFile2.aspx", "Admin File2 New", Encoding.UTF8);
            adminDirectory.CreateFile("AdminFile3.aspx", "Admin File3 V2", Encoding.UTF8);

            var themesDirectory = sourceDirectory.CreateSubDirectory("Themes");
            
            var defaultThemeDirectory = themesDirectory.CreateSubDirectory("Default");
            defaultThemeDirectory.CreateFile("reset.css", "Default Theme reset V1", Encoding.UTF8);
            defaultThemeDirectory.CreateFile("layout.css", "Default Theme layout New", Encoding.UTF8);
            defaultThemeDirectory.CreateFile("style.css", "Default Theme style V2", Encoding.UTF8);

            var defaultThemeImagesDirectory = defaultThemeDirectory.CreateSubDirectory("images");
            defaultThemeImagesDirectory.CreateFile("ico.png", "Default Theme ico V1", Encoding.UTF8);
            defaultThemeImagesDirectory.CreateFile("logo.jpg", "Default Theme logo V2", Encoding.UTF8);
            defaultThemeImagesDirectory.CreateFile("bg.gif", "Default Theme bg New", Encoding.UTF8);

            var currentChecksums = new List<FileChecksum>
            {
                new FileChecksum("/File2.aspx", ChecksumComputer.Compute("File2 V1", Encoding.UTF8)),
                new FileChecksum("/File3.aspx", ChecksumComputer.Compute("File3 V1", Encoding.UTF8)),
                new FileChecksum("/Admin/AdminFile1.aspx", ChecksumComputer.Compute("Admin File1 V1", Encoding.UTF8)),
                new FileChecksum("/Admin/AdminFile3.aspx", ChecksumComputer.Compute("Admin File3 V1", Encoding.UTF8)),
                new FileChecksum("/Themes/Default/reset.css", ChecksumComputer.Compute("Default Theme reset V1", Encoding.UTF8)),
                new FileChecksum("/Themes/Default/style.css", ChecksumComputer.Compute("Default Theme style V1", Encoding.UTF8)),
                new FileChecksum("/Themes/Default/images/icon.png", ChecksumComputer.Compute("Default Theme icon V1", Encoding.UTF8)),
                new FileChecksum("/Themes/Default/images/logo.jpg", ChecksumComputer.Compute("Default Theme logo V1", Encoding.UTF8))
            };

            var collector = new FileChangeCollector();
            var context = new FileChangeCollectionContext(sourceDirectory, currentChecksums, null);

            var changes = collector.Collect(context);

            var file1 = changes.FirstOrDefault(x => x.File.VirtualPath == "/File1.aspx");
            AssertFileChange(file1, true, ChecksumComputer.Compute("File1 New", Encoding.UTF8));
            Assert.Null(file1.OldChecksum);

            var file2 = changes.FirstOrDefault(x => x.File.VirtualPath == "/File2.aspx");
            AssertFileChange(file2, false, ChecksumComputer.Compute("File2 V2", Encoding.UTF8));
            Assert.Equal(ChecksumComputer.Compute("File2 V1", Encoding.UTF8), file2.OldChecksum.Checksum);

            Assert.Null(changes.FirstOrDefault(x => x.File.VirtualPath == "/File3.aspx"));

            Assert.Null(changes.FirstOrDefault(x => x.File.VirtualPath == "/Admin/AdminFile1.aspx"));

            var adminFile2 = changes.FirstOrDefault(x => x.File.VirtualPath == "/Admin/AdminFile2.aspx");
            AssertFileChange(adminFile2, true, ChecksumComputer.Compute("Admin File2 New", Encoding.UTF8));
            Assert.Null(adminFile2.OldChecksum);

            var adminFile3 = changes.FirstOrDefault(x => x.File.VirtualPath == "/Admin/AdminFile3.aspx");
            AssertFileChange(adminFile3, false, ChecksumComputer.Compute("Admin File3 V2", Encoding.UTF8));
            Assert.Equal(ChecksumComputer.Compute("Admin File3 V1", Encoding.UTF8), adminFile3.OldChecksum.Checksum);

            Assert.Null(changes.FirstOrDefault(x => x.File.VirtualPath == "/Themes/Default/reset.css"));

            var defaultThemeLayout = changes.FirstOrDefault(x => x.File.VirtualPath == "/Themes/Default/layout.css");
            AssertFileChange(defaultThemeLayout, true, ChecksumComputer.Compute("Default Theme layout New", Encoding.UTF8));
            Assert.Null(defaultThemeLayout.OldChecksum);

            var defaultThemeStyle = changes.FirstOrDefault(x => x.File.VirtualPath == "/Themes/Default/style.css");
            AssertFileChange(defaultThemeStyle, false, ChecksumComputer.Compute("Default Theme style V2", Encoding.UTF8));
            Assert.Equal(ChecksumComputer.Compute("Default Theme style V1", Encoding.UTF8), defaultThemeStyle.OldChecksum.Checksum);

            Assert.Null(changes.FirstOrDefault(x => x.File.VirtualPath == "/Themes/Default/images/icon.png"));

            var defaultThemeLogoImage = changes.FirstOrDefault(x => x.File.VirtualPath == "/Themes/Default/images/logo.jpg");
            AssertFileChange(defaultThemeLogoImage, false, ChecksumComputer.Compute("Default Theme logo V2", Encoding.UTF8));
            Assert.Equal(ChecksumComputer.Compute("Default Theme logo V1", Encoding.UTF8), defaultThemeLogoImage.OldChecksum.Checksum);

            var defaultThemeBgImage = changes.FirstOrDefault(x => x.File.VirtualPath == "/Themes/Default/images/bg.gif");
            AssertFileChange(defaultThemeBgImage, true, ChecksumComputer.Compute("Default Theme bg New", Encoding.UTF8));
            Assert.Null(defaultThemeBgImage.OldChecksum);
        }

        private void AssertFileChange(FileChange change, bool expectedIsNewFile, string expectedNewChecksum)
        {
            Assert.NotNull(change);
            Assert.Equal(expectedIsNewFile, change.IsNewFile);
            Assert.NotNull(change.NewChecksum);
            Assert.Equal(change.NewChecksum.Checksum, expectedNewChecksum);
        }

        class MockIgnorantRule : AbstractIgnorantRule
        {
            public List<string> IgnoredFileVirtualPaths { get; set; }

            public List<string> IgnoredDirectoryVirtualPaths { get; set; }

            public override bool ShouldIgnore(XDeploy.IO.IFileSystemInfo info, FileChangeCollectionContext context)
            {
                if (info is IFile)
                {
                    return IgnoredDirectoryVirtualPaths != null && IgnoredFileVirtualPaths.Any(x => x.Equals(info.VirtualPath, StringComparison.OrdinalIgnoreCase));
                }

                return IgnoredDirectoryVirtualPaths != null && IgnoredDirectoryVirtualPaths.Any(x => x.Equals(info.VirtualPath, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
