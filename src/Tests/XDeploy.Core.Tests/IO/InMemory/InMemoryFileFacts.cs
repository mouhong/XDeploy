using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDeploy.IO.InMemory;
using Xunit;

namespace XDeploy.Core.Tests.IO.InMemory
{
    public class InMemoryFileFacts
    {
        public class ThePathRelatedProperties
        {
            [Fact]
            public void CanReturnBaseOnDirectoryPaths()
            {
                var directory = new InMemoryDirectory("/Root", "D:\\Root", null);
                var file = new InMemoryFile("file1.txt", directory);
                Assert.Equal("/Root/file1.txt", file.VirtualPath);
                Assert.Equal("D:\\Root\\file1.txt", file.Uri);
            }
        }

        public class TheExistsProperty
        {
            [Fact]
            public void WillBeTrueWhenStreamIsNullAndViceVersa()
            {
                var directory = new InMemoryDirectory("/Root", "D:\\Root", null);
                var file = new InMemoryFile("file.txt", directory, null);
                Assert.False(file.Exists);
                file.WriteAllTexts("Hello", Encoding.UTF8);
                Assert.True(file.Exists);
            }
        }

        public class TheDeleteMethod
        {
            [Fact]
            public void WillClearFileContentAndMakeExistsPropertyToReturnFalse()
            {
                var directory = new InMemoryDirectory("/Root", "D:\\Root", null);
                var file = new InMemoryFile("file1.txt", directory);
                file.WriteAllTexts("Hello", Encoding.UTF8);

                Assert.NotNull(file.Data);
                Assert.True(file.Exists);

                file.Delete();

                Assert.Null(file.Data);
                Assert.False(file.Exists);
            }
        }
    }
}
