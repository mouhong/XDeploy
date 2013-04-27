using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDeploy.Storage;

namespace XDeploy
{
    public interface IBackupFolderNameGenerator
    {
        string Generate(IDirectory containingDirectory);
    }
}
