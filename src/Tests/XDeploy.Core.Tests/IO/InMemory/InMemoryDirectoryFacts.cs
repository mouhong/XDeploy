using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XDeploy.IO.InMemory;
using Xunit;

namespace XDeploy.Core.Tests.IO.InMemory
{
    public class InMemoryDirectoryFacts
    {
        public class TheCreateMethod
        {
            [Fact]
            public void WillSetExistsToTrue()
            {
                var directory = new InMemoryDirectory("/Root", "D:\\Root", null);
                Assert.False(directory.Exists);
                directory.Create();
                Assert.True(directory.Exists);
            }
        }

        public class TheCreateSubDirectoryMethod
        {
            [Fact]
            public void WillCreateDirectoryWithPropertiesCorrectlySet()
            {
                var directory = new InMemoryDirectory("/Root", "D:\\Root", null);
                var sub = directory.CreateSubDirectory("Sub");
                Assert.NotNull(sub);
                Assert.True(sub.Exists);
                Assert.Same(sub.Parent, directory);
                Assert.Equal("Sub", sub.Name);
                Assert.Equal("/Root/Sub", sub.VirtualPath);
                Assert.Equal("D:\\Root\\Sub", sub.Uri);
            }
        }

        public class TheCreateFileMethod
        {
            [Fact]
            public void CanCreateFileWithPropertiesCorrectlySet()
            {
                var directory = new InMemoryDirectory("/Root", "D:\\Root", null);
                var file = directory.CreateFile("myfile.txt", "Hello", Encoding.UTF8);

                Assert.Equal(1, directory.GetFileSystemInfos().Count());
                Assert.Same(file, directory.GetFileSystemInfos().First());
                Assert.Same(directory, file.Directory);
                Assert.True(file.Exists);
                Assert.True(directory.Exists);
                Assert.Equal("myfile.txt", file.Name);
                Assert.Equal("/Root/myfile.txt", file.VirtualPath);
                Assert.Equal("D:\\Root\\myfile.txt", file.Uri);

                var reader = new StreamReader(file.Stream, Encoding.UTF8);
                Assert.Equal("Hello", reader.ReadToEnd());
            }
        }

        public class TheGetFileMethod
        {
            [Fact]
            public void CanHandleFileName()
            {
                var directory = new InMemoryDirectory("/Root", "D:\\Root", null);
                var file = directory.GetFile("file.zip");
                Assert.NotNull(file);
                Assert.Equal("file.zip", file.Name);
                Assert.Equal("/Root/file.zip", file.VirtualPath);
                Assert.Equal("D:\\Root\\file.zip", file.Uri);
            }

            [Fact]
            public void CanHandleRelativeVirtualPath()
            {
                var directory = new InMemoryDirectory("/Root", "D:\\Root", null);
                var file = directory.GetFile("Sub/file.sql");
                Assert.NotNull(file);
                Assert.Equal("file.sql", file.Name);
                Assert.Equal("/Root/Sub/file.sql", file.VirtualPath);
                Assert.Equal("D:\\Root\\Sub\\file.sql", file.Uri);

                file = directory.GetFile("Sub/Sub2/file2.cs");
                Assert.NotNull(file);
                Assert.Equal("file2.cs", file.Name);
                Assert.Equal("/Root/Sub/Sub2/file2.cs", file.VirtualPath);
                Assert.Equal("D:\\Root\\Sub\\Sub2\\file2.cs", file.Uri);
            }

            [Fact]
            public void WhenGetNotExistFile_WillNeverReturnNullButReturnFileWithExistsSetToFalse()
            {
                var directory = new InMemoryDirectory("/Root", "D:\\Root", null);
                var file = directory.GetFile("myfile.txt");
                Assert.NotNull(file);
                Assert.False(file.Exists);

                file = directory.GetFile("Sub/myfile2.txt");
                Assert.NotNull(file);
                Assert.False(file.Exists);
            }
        }

        public class TheGetDirectoryMethod
        {
            [Fact]
            public void CanHandleDirectoryName()
            {
                var directory = new InMemoryDirectory("/Root", "D:\\Root", null);
                var sub = directory.GetFile("Sub");
                Assert.NotNull(sub);
                Assert.Equal("Sub", sub.Name);
                Assert.Equal("/Root/Sub", sub.VirtualPath);
                Assert.Equal("D:\\Root\\Sub", sub.Uri);
            }

            [Fact]
            public void CanHandleRelativeVirtualPath()
            {
                var directory = new InMemoryDirectory("/Root", "D:\\Root", null);
                var sub = directory.GetFile("Sub/Sub2");
                Assert.NotNull(sub);
                Assert.Equal("Sub2", sub.Name);
                Assert.Equal("/Root/Sub/Sub2", sub.VirtualPath);
                Assert.Equal("D:\\Root\\Sub\\Sub2", sub.Uri);

                sub = directory.GetFile("Sub/Sub2/Sub3");
                Assert.NotNull(sub);
                Assert.Equal("Sub3", sub.Name);
                Assert.Equal("/Root/Sub/Sub2/Sub3", sub.VirtualPath);
                Assert.Equal("D:\\Root\\Sub\\Sub2\\Sub3", sub.Uri);
            }

            [Fact]
            public void WhenGetNotExistDirectory_WillNeverReturnNullButReturnDirectoryWithExistsSetToFalse()
            {
                var directory = new InMemoryDirectory("/Root", "D:\\Root", null);
                var sub = directory.GetFile("Sub");
                Assert.NotNull(sub);
                Assert.False(sub.Exists);

                sub = directory.GetFile("Sub/Sub2");
                Assert.NotNull(sub);
                Assert.False(sub.Exists);
            }
        }

        public class TheGetFilesMethod
        {
            [Fact]
            public void WillOnlyReturnExistingFiles()
            {
                var directory = new InMemoryDirectory("/Root", "D:\\Root", null);
                directory.CreateFile("file1.txt", "file1 content", Encoding.UTF8);
                directory.CreateFile("file2.txt", "file2 content", Encoding.UTF8);

                directory.GetFile("file3.txt");

                var files = directory.GetFiles().ToList();
                Assert.Equal(2, files.Count);
                Assert.Equal("file1.txt", files[0].Name);
                Assert.Equal("file2.txt", files[1].Name);
            }
        }

        public class TheGetDirectoriesMethod
        {
            [Fact]
            public void WillOnlyReturnExistingDirectories()
            {
                var directory = new InMemoryDirectory("/Root", "D:\\Root", null);
                directory.CreateSubDirectory("Sub1");
                directory.CreateSubDirectory("Sub2");

                directory.GetDirectory("Sub3");

                var subDirectories = directory.GetDirectories().ToList();
                Assert.Equal(2, subDirectories.Count);
                Assert.Equal("Sub1", subDirectories[0].Name);
                Assert.Equal("Sub2", subDirectories[1].Name);
            }
        }
    }
}
