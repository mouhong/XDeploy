using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using XDeploy.Utils;

namespace XDeploy.Storage.Local
{
    public class LocalDirectory : IDirectory
    {
        public DirectoryInfo WrappedDirectory { get; private set; }

        public string Uri
        {
            get
            {
                return WrappedDirectory.FullName;
            }
        }

        public string VirtualPath { get; private set; }

        public bool IsRoot
        {
            get
            {
                return VirtualPath == "/";
            }
        }

        public bool Exists { get; private set; }

        public LocalDirectory(string virtualPath, DirectoryInfo directory)
        {
            VirtualPath = virtualPath;
            WrappedDirectory = directory;
            Exists = directory.Exists;
        }

        public IFile GetFile(string relativeVirtualPath)
        {
            return new LocalFile(GetFullVirtualPath(relativeVirtualPath), new FileInfo(GetPhysicalPath(relativeVirtualPath)));
        }

        public IDirectory GetDirectory(string relativeVirtualPath)
        {
            return new LocalDirectory(GetFullVirtualPath(relativeVirtualPath), new DirectoryInfo(GetPhysicalPath(relativeVirtualPath)));
        }

        public void Create()
        {
            if (!Exists)
            {
                WrappedDirectory.Create();
                Exists = true;
            }
        }

        public void Refresh()
        {
            WrappedDirectory.Refresh();
        }

        private string GetPhysicalPath(string virtualPath)
        {
            var path = virtualPath.Replace('/', '\\').TrimStart('\\');
            return Path.Combine(WrappedDirectory.FullName, path);
        }

        private string GetFullVirtualPath(string relativeVirtualPath)
        {
            return VirtualPathUtil.Combine(VirtualPath, relativeVirtualPath);
        }

        public void Dispose()
        {
        }
    }
}
