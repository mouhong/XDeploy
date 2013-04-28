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
        public string Uri { get; private set; }

        public string VirtualPath { get; private set; }

        public LazyFtpClient FtpClient { get; private set; }

        public bool Exists
        {
            get
            {
                return FtpClient.FileExists(Uri);
            }
        }

        public FtpFile(string virtualPath, Uri uri, NetworkCredential credential)
            : this(virtualPath, uri, new LazyFtpClient(uri.Host, credential, uri.Port))
        {
        }

        public FtpFile(string virtualPath, Uri uri, LazyFtpClient ftpClient)
        {
            VirtualPath = virtualPath;
            Uri = uri.OriginalString;
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
            FtpClient.DeleteFile(Uri);
        }

        public Stream OpenRead()
        {
            return FtpClient.OpenRead(Uri);
        }

        public Stream OpenWrite()
        {
            return FtpClient.OpenWrite(Uri);
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
