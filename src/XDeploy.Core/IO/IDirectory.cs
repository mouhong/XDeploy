using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace XDeploy.IO
{
    public interface IDirectory : IFileSystemInfo, IDisposable
    {
        bool IsRoot { get; }

        void Create();

        IFile GetFile(string relativeVirtualPath);

        IDirectory GetDirectory(string relativeVirtualPath);

        IEnumerable<IFile> GetFiles();

        IEnumerable<IDirectory> GetDirectories();

        IEnumerable<IFileSystemInfo> GetFileSystemInfos();
    }
}
