using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XDeploy.Utils;

namespace XDeploy.IO.InMemory
{
    public class InMemoryFile : IFile
    {
        public Stream Stream { get; private set; }

        public string Name { get; private set; }

        public string Uri
        {
            get
            {
                return Path.Combine(Directory.Uri, Name);
            }
        }

        public string VirtualPath
        {
            get
            {
                return VirtualPathUtil.Combine(Directory.VirtualPath, Name);
            }
        }

        public string Extension
        {
            get
            {
                return Path.GetExtension(VirtualPath);
            }
        }

        public bool Exists
        {
            get
            {
                return Stream != null;
            }
        }

        public long Length
        {
            get
            {
                return Stream.Length;
            }
        }

        public InMemoryDirectory Directory { get; private set; }

        public InMemoryFile(string fileName, InMemoryDirectory directory, Stream contents = null)
        {
            Require.NotNullOrEmpty(fileName, "fileName");
            Require.NotNull(directory, "directory");

            Name = fileName;
            Directory = directory;
            Stream = contents;
        }

        public void Delete()
        {
            if (Stream != null)
            {
                Stream.Dispose();
                Stream = null;
            }
        }

        public IDirectory GetDirectory()
        {
            return Directory;
        }

        public IDirectory CreateDirectory()
        {
            Directory.Create();
            return Directory;
        }

        public System.IO.Stream OpenRead()
        {
            if (Stream == null)
                throw new IOException("File not exists.");

            Stream.Position = 0;
            return Stream;
        }

        public System.IO.Stream OpenWrite()
        {
            EnsureCreated();
            Stream.Position = 0;
            return Stream;
        }

        public void WriteAllTexts(string text, Encoding encoding)
        {
            Stream = new MemoryStream();
            var bytes = encoding.GetBytes(text);
            Stream.Write(bytes, 0, bytes.Length);
            Stream.Position = 0;
        }

        public void Rename(string newFileName)
        {
            Name = newFileName;
        }

        public void Refresh()
        {
        }

        private void EnsureCreated()
        {
            if (Stream == null)
            {
                Stream = new MemoryStream();
            }
        }
    }
}
