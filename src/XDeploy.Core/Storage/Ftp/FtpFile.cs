using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using XDeploy.Utils;

namespace XDeploy.Storage.Ftp
{
    public class FtpFile : IFile
    {
        private FtpFileCache _cache;

        private FtpFileCache Cache
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

        public string Uri { get; private set; }

        public string VirtualPath { get; private set; }

        public string AbsolutePathInFtp { get; private set; }

        public LazyFtpClient FtpClient { get; private set; }

        public bool Exists
        {
            get
            {
                return Cache.Exists;
            }
        }

        public FtpFile(string virtualPath, Uri uri, NetworkCredential credential)
            : this(virtualPath, uri, new LazyFtpClient(uri.Host, credential, uri.Port))
        {
        }

        public FtpFile(string virtualPath, Uri uri, LazyFtpClient ftpClient)
        {
            VirtualPath = virtualPath;
            Uri = uri.ToString();
            AbsolutePathInFtp = uri.AbsolutePath;
            FtpClient = ftpClient;
        }

        public IDirectory GetDirectory()
        {
            return new FtpDirectory(VirtualPathUtil.GetDirectory(VirtualPath), new Uri(VirtualPathUtil.GetDirectory(Uri)), FtpClient);
        }

        public IDirectory CreateDirectory()
        {
            var directory = GetDirectory();
            directory.Create();
            return directory;
        }

        public void Delete()
        {
            if (Exists)
            {
                EnsureConnected();
                FtpClient.DeleteFile(AbsolutePathInFtp);
            }
        }

        public Stream OpenRead()
        {
            EnsureConnected();
            return FtpClient.OpenRead(AbsolutePathInFtp);
        }

        public Stream OpenWrite()
        {
            EnsureConnected();
            return FtpClient.OpenWrite(AbsolutePathInFtp);
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

        public void Refresh()
        {
            EnsureConnected();

            var cache = new FtpFileCache();
            cache.Exists = FtpClient.FileExists(AbsolutePathInFtp);

            _cache = cache;
        }

        private void EnsureConnected()
        {
            if (!FtpClient.IsConnected)
            {
                FtpClient.Connect();
            }
        }

        class FtpFileCache
        {
            public bool Exists = false;
        }
    }
}
