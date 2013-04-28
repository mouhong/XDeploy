using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace XDeploy.Storage
{
    public interface IDirectory : IDisposable
    {
        string FullName { get; }

        NetworkCredential Credential { get; set; }

        void ClearDirectory(string virtualPath);

        void EnsureDirectoryCreated(string virtualPath);

        bool FileExists(string virtualPath);

        bool DirectoryExists(string virtualPath);

        Stream OpenRead(string virtualPath);

        Stream OpenWrite(string virtualPath);

        void StoreFile(FileInfo file, string fileVirtualPath, bool overwrite);

        IDirectory GetDirectory(string virtualPath);
    }
}
