﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace XDeploy.IO
{
    public interface IFile
    {
        string Name { get; }

        string Uri { get; }

        string VirtualPath { get; }

        bool Exists { get; }

        long Length { get; }

        void Delete();

        void Refresh();

        IDirectory GetDirectory();

        IDirectory CreateDirectory();

        Stream OpenRead();

        Stream OpenWrite();

        void Rename(string newFileName);
    }
}