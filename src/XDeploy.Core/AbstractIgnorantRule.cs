using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XDeploy
{
    public abstract class AbstractIgnorantRule
    {
        public abstract bool ShouldIgnore(FileSystemInfo info, DirectoryInfo rootDirectory);
    }
}
