using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using XDeploy.Changes;
using XDeploy.IO;

namespace XDeploy
{
    public abstract class AbstractIgnorantRule
    {
        public abstract bool ShouldIgnore(IFileSystemInfo info, FileChangeCollectionContext context);
    }
}
