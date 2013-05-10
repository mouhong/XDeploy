using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace XDeploy.IO
{
    public interface IFile : IFileSystemInfo
    {
        long Length { get; }

        void Delete();

        IDirectory GetDirectory();

        IDirectory CreateDirectory();

        Stream OpenRead();

        Stream OpenWrite();

        void Rename(string newFileName);
    }
}
