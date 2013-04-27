using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public class LastWriteTimeIgnorantRule : IIgnorantRule
    {
        public DateTime IgnoreWhenUtcLastWriteTimeBeforeThisValue { get; set; }

        public LastWriteTimeIgnorantRule()
        {
            IgnoreWhenUtcLastWriteTimeBeforeThisValue = DateTime.MinValue.ToUniversalTime();
        }
        
        public bool ShouldIgnore(FileSystemInfo info, DirectoryInfo rootDirectory)
        {
            if (info.LastWriteTimeUtc < IgnoreWhenUtcLastWriteTimeBeforeThisValue)
            {
                return true;
            }

            return false;
        }
    }
}
