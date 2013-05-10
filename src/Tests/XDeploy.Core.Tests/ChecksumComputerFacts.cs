using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace XDeploy.Core.Tests
{
    public class ChecksumComputerFacts
    {
        [Fact]
        public void WillNeverGetNullOrEmpty()
        {
            var checksum = ChecksumComputer.Compute(CreateStream("hello"));
            Assert.True(!String.IsNullOrWhiteSpace(checksum));
        }

        [Fact]
        public void SameStreamWillGetSameChecksum()
        {
            var checksum1 = ChecksumComputer.Compute(CreateStream("hello world!"));
            var checksum2 = ChecksumComputer.Compute(CreateStream("hello world!"));

            Assert.Equal(checksum1, checksum2);
        }

        [Fact]
        public void DifferentStreamWillGetDifferentChecksum()
        {
            var checksum1 = ChecksumComputer.Compute(CreateStream("hello world!"));
            var checksum2 = ChecksumComputer.Compute(CreateStream("hello  world!"));

            Assert.NotEqual(checksum1, checksum2);
        }

        [Fact]
        public void WillBeCaseSensitive()
        {
            var checksum1 = ChecksumComputer.Compute(CreateStream("hello world"));
            var checksum2 = ChecksumComputer.Compute(CreateStream("Hello World"));
            Assert.NotEqual(checksum1, checksum2);
        }

        [Fact]
        public void FilesWithDifferentEncodingWillGetDifferentChecksum()
        {
            var file1 = CreateTempFile("hello world", Encoding.UTF8);
            var file2 = CreateTempFile("hello world", Encoding.GetEncoding("GB2312"));

            Assert.NotEqual(ChecksumComputer.Compute(file1), ChecksumComputer.Compute(file2));

            file1.Delete();
            file2.Delete();
        }

        [Fact]
        public void FilesWithSameContentButLastWriteTimeNotSame_WillGetSameChecksum()
        {
            var file1 = CreateTempFile("hello world!", Encoding.UTF8);
            var file2 = CreateTempFile("hello world!", Encoding.UTF8);

            file2.LastWriteTime = DateTime.Now.AddHours(-1);

            Assert.Equal(ChecksumComputer.Compute(file1), ChecksumComputer.Compute(file2));

            file1.Delete();
            file2.Delete();
        }

        private FileInfo CreateTempFile(string contents, Encoding encoding)
        {
            var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
            var filePath = Path.Combine(basePath, Guid.NewGuid() + ".txt");
            File.WriteAllText(filePath, contents, encoding);
            return new FileInfo(filePath);
        }

        private Stream CreateStream(string content)
        {
            var stream = new MemoryStream();
            var bytes = Encoding.UTF8.GetBytes(content);
            stream.Write(bytes, 0, bytes.Length);
            stream.Position = 0;
            return stream;
        }
    }
}
