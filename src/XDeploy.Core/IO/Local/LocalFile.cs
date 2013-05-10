using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XDeploy.Utils;

namespace XDeploy.IO.Local
{
    public class LocalFile : IFile
    {
        public string Name
        {
            get
            {
                return WrappedFile.Name;
            }
        }

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

        public string Extension
        {
            get
            {
                return WrappedFile.Extension;
            }
        }

        public long Length
        {
            get
            {
                return WrappedFile.Length;
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
            if (!directory.Exists)
            {
                directory.Create();
            }

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

        public Stream CreateOrOpenWrite()
        {
            if (!WrappedFile.Exists)
            {
                CreateDirectory();
                return WrappedFile.Create();
            }

            return WrappedFile.OpenWrite();
        }

        public void Rename(string newFileName)
        {
            Require.NotNullOrEmpty(newFileName, "newFileName");
            WrappedFile.MoveTo(Path.Combine(Path.GetDirectoryName(Uri), newFileName));
        }

        public override string ToString()
        {
            return Uri;
        }
    }
}
