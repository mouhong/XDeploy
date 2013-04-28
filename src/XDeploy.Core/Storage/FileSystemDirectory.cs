using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace XDeploy.Storage
{
    public class FileSystemDirectory : IDirectory
    {
        public NetworkCredential Credential { get; set; }

        public DirectoryInfo RootDirectory { get; private set; }

        public string FullName
        {
            get
            {
                return RootDirectory.FullName;
            }
        }

        public FileSystemDirectory(string rootDirectory)
            : this(new DirectoryInfo(rootDirectory))
        {
        }

        public FileSystemDirectory(DirectoryInfo rootDirectory)
        {
            Require.NotNull(rootDirectory, "rootDirectory");
            RootDirectory = rootDirectory;
        }

        public IDirectory GetDirectory(string virtualPath)
        {
            return new FileSystemDirectory(GetPhysicalPath(virtualPath))
            {
                Credential = Credential
            };
        }

        public bool FileExists(string virtualPath)
        {
            Require.NotNullOrEmpty(virtualPath, "virtualPath");
            return File.Exists(GetPhysicalPath(virtualPath));
        }

        public bool DirectoryExists(string virtualPath)
        {
            Require.NotNullOrEmpty(virtualPath, "virtualPath");
            return Directory.Exists(GetPhysicalPath(virtualPath));
        }

        public Stream OpenRead(string virtualPath)
        {
            Require.NotNullOrEmpty(virtualPath, "virtualPath");
            return File.OpenRead(GetPhysicalPath(virtualPath));
        }

        public Stream OpenWrite(string virtualPath)
        {
            Require.NotNullOrEmpty(virtualPath, "virtualPath");

            EnsureDirectoryCreated(Path.GetDirectoryName(virtualPath));

            return File.OpenWrite(GetPhysicalPath(virtualPath));
        }

        public void ClearDirectory(string virtualPath)
        {
            Require.NotNullOrEmpty(virtualPath, "virtualPath");

            var directory = new DirectoryInfo(GetPhysicalPath(virtualPath));

            if (directory.Exists)
            {
                foreach (var entry in directory.GetFileSystemInfos())
                {
                    if (entry.Exists)
                        entry.Delete();
                }
            }
        }

        public void EnsureDirectoryCreated(string virtualPath)
        {
            Require.NotNullOrEmpty(virtualPath, "virtualPath");

            var directory = new DirectoryInfo(GetPhysicalPath(virtualPath));
            if (!directory.Exists)
            {
                directory.Create();
            }
        }

        public void StoreFile(FileInfo file, string fileVirtualPath, bool overwrite)
        {
            Require.NotNull(file, "file");
            Require.NotNullOrEmpty(fileVirtualPath, "fileVirtualPath");

            var physicalPath = GetPhysicalPath(fileVirtualPath);
            file.CopyTo(physicalPath, overwrite);
        }

        private string GetPhysicalPath(string virtualPath)
        {
            var path = virtualPath.Replace('/', '\\').TrimStart('\\');
            return Path.Combine(RootDirectory.FullName, path);
        }

        public void Dispose()
        {
        }
    }
}
