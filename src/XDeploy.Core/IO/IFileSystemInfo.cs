using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.IO
{
    public interface IFileSystemInfo
    {
        string Name { get; }

        string Uri { get; }

        string VirtualPath { get; }

        string Extension { get; }

        bool Exists { get; }

        void Refresh();
    }
}
