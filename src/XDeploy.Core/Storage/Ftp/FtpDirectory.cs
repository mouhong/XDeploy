using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.FtpClient;
using XDeploy.Utils;
using System.IO;

namespace XDeploy.Storage.Ftp
{
    public class FtpDirectory : IDirectory
    {
        public LazyFtpClient FtpClient { get; private set; }

        public string VirtualPath { get; private set; }

        public string Uri { get; private set; }

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
                return FtpClient.DirectoryExists(VirtualPath);
            }
        }

        public FtpDirectory(string virtualPath, Uri uri, NetworkCredential credential)
            : this(virtualPath, uri, new LazyFtpClient(uri.Host, credential, uri.Port))
        {
        }

        public FtpDirectory(string virtualPath, Uri uri, LazyFtpClient ftpClient)
        {
            VirtualPath = virtualPath;
            Uri = uri.OriginalString;
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

        public void Create()
        {
            EnsureConnected();
            FtpClient.CreateDirectory(Uri, true);
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
    }
}
