using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.FtpClient;
using XDeploy.Utils;
using System.IO;

namespace XDeploy.IO.Ftp
{
    public class FtpDirectory : IDirectory
    {
        private FtpDirectoryCache _cache;

        private FtpDirectoryCache Cache
        {
            get
            {
                if (_cache == null)
                {
                    Refresh();
                }
                return _cache;
            }
        }

        public LazyFtpClient FtpClient { get; private set; }

        public string Name
        {
            get
            {
                return Path.GetFileName(VirtualPath);
            }
        }

        public string VirtualPath { get; private set; }

        public string Uri { get; private set; }

        public string AbsolutePathInFtp { get; private set; }

        public bool IsRoot
        {
            get
            {
                return VirtualPath == "/";
            }
        }

        public bool Exists
        {
            get
            {
                return Cache.Exists;
            }
        }

        public string Extension
        {
            get
            {
                return Path.GetExtension(VirtualPath);
            }
        }

        public FtpDirectory(string virtualPath, Uri uri, NetworkCredential credential)
            : this(virtualPath, uri, new LazyFtpClient(uri.Host, credential, uri.Port))
        {
        }

        public FtpDirectory(string virtualPath, Uri uri, LazyFtpClient ftpClient)
        {
            VirtualPath = virtualPath;
            Uri = uri.ToString();
            AbsolutePathInFtp = uri.AbsolutePath;
            FtpClient = ftpClient;
        }

        public IFile GetFile(string relativeVirtualPath)
        {
            var fullVirtualPath = VirtualPathUtil.Combine(VirtualPath, relativeVirtualPath);
            return new FtpFile(fullVirtualPath, new Uri(VirtualPathUtil.Combine(Uri, relativeVirtualPath)), FtpClient);
        }

        public IDirectory GetDirectory(string relativeVirtualPath)
        {
            var fullVirtualPath = VirtualPathUtil.Combine(VirtualPath, relativeVirtualPath);
            return new FtpDirectory(fullVirtualPath, new Uri(VirtualPathUtil.Combine(Uri, relativeVirtualPath)), FtpClient);
        }

        public IEnumerable<IFile> GetFiles()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDirectory> GetDirectories()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IFileSystemInfo> GetFileSystemInfos()
        {
            throw new NotImplementedException();
        }

        public void Create()
        {
            if (!Exists)
            {
                EnsureConnected();
                FtpClient.CreateDirectory(AbsolutePathInFtp, true);
            }
        }

        public void Refresh()
        {
            EnsureConnected();

            var cache = new FtpDirectoryCache();
            cache.Exists = FtpClient.DirectoryExists(AbsolutePathInFtp);

            _cache = cache;
        }

        private void EnsureConnected()
        {
            if (!FtpClient.IsConnected)
            {
                FtpClient.Connect();
            }
        }

        public void Dispose()
        {
            FtpClient.Dispose();
        }

        class FtpDirectoryCache
        {
            public bool Exists = false;
        }
    }
}
