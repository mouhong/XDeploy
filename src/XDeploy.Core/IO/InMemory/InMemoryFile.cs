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
        public byte[] Data { get; set; }

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
                return Data != null;
            }
        }

        public long Length
        {
            get
            {
                return Data == null ? 0 : Data.Length;
            }
        }

        public InMemoryDirectory Directory { get; private set; }

        public InMemoryFile(string fileName, InMemoryDirectory directory, byte[] data = null)
        {
            Require.NotNullOrEmpty(fileName, "fileName");
            Require.NotNull(directory, "directory");

            Name = fileName;
            Directory = directory;
            Data = data;
        }

        public void Delete()
        {
            Data = null;
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
            if (Data == null)
                throw new IOException("File not exists.");

            var stream = new MemoryStream();
            stream.Write(Data, 0, Data.Length);
            stream.Position = 0;

            return stream;
        }

        public System.IO.Stream CreateOrOpenWrite()
        {
            var stream = new MemoryStream();
            stream.Write(Data, 0, Data.Length);
            stream.Position = 0;

            return stream;
        }

        public void WriteAllTexts(string text, Encoding encoding)
        {
            Data = encoding.GetBytes(text);
        }

        public void Rename(string newFileName)
        {
            Name = newFileName;
        }

        public void Refresh()
        {
        }

        public override string ToString()
        {
            return Uri;
        }
    }
}
