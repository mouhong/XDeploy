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
        string Uri { get; }

        string VirtualPath { get; }

        bool Exists { get; }

        bool IsRoot { get; }

        void Refresh();

        void Create();

        IFile GetFile(string relativeVirtualPath);

        IDirectory GetDirectory(string relativeVirtualPath);
    }
}
