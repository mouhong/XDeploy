using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XDeploy.Utils;

namespace XDeploy.Storage.Local
{
    public class LocalFile : IFile
    {
        public string Uri
        {
            get
            {
                return WrappedFile.FullName;
            }
        }

        public string VirtualPath { get; private set; }

        public FileInfo WrappedFile { get; private set; }

        public bool Exists
        {
            get
            {
                return WrappedFile.Exists;
            }
        }

        public LocalFile(string virtualPath, FileInfo file)
        {
            VirtualPath = virtualPath;
            WrappedFile = file;
        }

        public IDirectory GetDirectory()
        {
            return new LocalDirectory(VirtualPathUtil.GetDirectory(VirtualPath), WrappedFile.Directory);
        }

        public IDirectory CreateDirectory()
        {
            var directory = GetDirectory();
            directory.Create();
            return directory;
        }

        public void Delete()
        {
            WrappedFile.Delete();
        }

        public void Refresh()
        {
            WrappedFile.Refresh();
        }

        public Stream OpenRead()
        {
            return WrappedFile.OpenRead();
        }

        public Stream OpenWrite()
        {
            return WrappedFile.OpenWrite();
        }

        public void OverwriteWith(Stream stream)
        {
            CreateDirectory();

            using (var thisStream = OpenWrite())
            {
                stream.WriteTo(thisStream);
                thisStream.Flush();
            }
        }

        public void OverwriteWith(IFile file)
        {
            using (var stream = file.OpenRead())
            {
                OverwriteWith(stream);
            }
        }
    }
}
