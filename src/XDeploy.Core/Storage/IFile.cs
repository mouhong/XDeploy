﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace XDeploy.Storage
{
    public interface IFile
    {
        string Uri { get; }

        string VirtualPath { get; }

        bool Exists { get; }

        void Delete();

        void Refresh();

        IDirectory GetDirectory();

        IDirectory CreateDirectory();

        Stream OpenRead();

        Stream OpenWrite();

        void OverwriteWith(Stream stream);

        void OverwriteWith(IFile file);
    }
}