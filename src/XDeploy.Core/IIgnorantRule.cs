using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public interface IIgnorantRule
    {
        bool ShouldIgnore(FileSystemInfo info, DirectoryInfo rootDirectory);
    }
}
