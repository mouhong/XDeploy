using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.FtpClient;
using XDeploy.Utils;
using System.IO;

namespace XDeploy.Storage
{
    public class FtpStorageLocation : IStorageLocation
    {
        private FtpClient _ftpClient;

        private FtpClient FtpClient
        {
            get
            {
                if (_ftpClient == null)
                {
                    var client = new FtpClient
                    {
                        Credentials = (NetworkCredential)Credentials,
                        Host = Host,
                        Port = Port
                    };

                    _ftpClient = client;
                }

                return _ftpClient;
            }
        }

        public ICredentials Credentials { get; set; }

        public string Host { get; private set; }

        public int Port { get; private set; }

        public string RootPath { get; private set; }

        public string FullName
        {
            get
            {
                return "ftp://" + Host + ":" + Port + RootPath;
            }
        }

        public FtpStorageLocation(string rootUri)
            : this(new Uri(rootUri))
        {
        }

        public FtpStorageLocation(Uri rootUri)
        {
            Require.NotNull(rootUri, "rootUri");

            Host = rootUri.Host;
            Port = rootUri.Port;
            RootPath = rootUri.AbsolutePath;
        }

        public IStorageLocation GetLocation(string virtualPath)
        {
            var location = new FtpStorageLocation(VirtualPathUtil.Combine(FullName, virtualPath))
            {
                Credentials = Credentials
            };
            location._ftpClient = _ftpClient;

            return location;
        }

        public bool FileExists(string virtualPath)
        {
            Require.NotNullOrEmpty(virtualPath, "virtualPath");

            EnsureConnected();

            return FtpClient.FileExists(GetFullPath(virtualPath), FtpListOption.ForceList | FtpListOption.AllFiles);
        }

        public bool DirectoryExists(string virtualPath)
        {
            Require.NotNullOrEmpty(virtualPath, "virtualPath");

            EnsureConnected();

            return FtpClient.DirectoryExists(GetFullPath(virtualPath));
        }

        public Stream OpenRead(string virtualPath)
        {
            Require.NotNullOrEmpty(virtualPath, "virtualPath");
            
            EnsureConnected();

            return FtpClient.OpenRead(GetFullPath(virtualPath));
        }

        public Stream OpenWrite(string virtualPath)
        {
            Require.NotNullOrEmpty(virtualPath, "virtualPath");

            EnsureConnected();

            return FtpClient.OpenWrite(GetFullPath(virtualPath));
        }

        public void ClearDirectory(string virtualPath)
        {
            Require.NotNullOrEmpty(virtualPath, "virtualPath");

            EnsureConnected();

            FtpClient.DeleteDirectory(GetFullPath(virtualPath), true);
        }

        public void EnsureDirectoryCreated(string virtualPath)
        {
            Require.NotNullOrEmpty(virtualPath, "virtualPath");
            
            EnsureConnected();

            FtpClient.CreateDirectory(GetFullPath(virtualPath), true);
        }

        public void StoreFile(FileInfo file, string fileVirtualPath, bool overwrite)
        {
            Require.NotNull(file, "file");
            Require.NotNullOrEmpty(fileVirtualPath, "fileVirtualPath");

            var fullPath = GetFullPath(fileVirtualPath);

            EnsureConnected();

            using (var fromStream = file.OpenRead())
            using (var toStream = FtpClient.OpenWrite(fullPath, FtpDataType.Binary))
            {
                var count = 0;
                var buffer = new byte[2048];

                while (true)
                {
                    count = fromStream.Read(buffer, 0, buffer.Length);

                    if (count > 0)
                    {
                        toStream.Write(buffer, 0, count);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private string GetFullPath(string virtualPath)
        {
            return VirtualPathUtil.Combine(RootPath, virtualPath);
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
            if (_ftpClient != null)
            {
                _ftpClient.Dispose();
            }
        }
    }
}
