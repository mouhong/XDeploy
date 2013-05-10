using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XDeploy.Changes;
using XDeploy.IO;

namespace XDeploy
{
    public class PathIgnorantRule : AbstractIgnorantRule
    {
        private string _ignoredPaths;
        private List<string> _ignoredPathList;

        public string IgnoredPaths
        {
            get
            {
                return _ignoredPaths;
            }
            set
            {
                if (_ignoredPaths != value)
                {
                    _ignoredPaths = value;
                    _ignoredPathList = (_ignoredPaths ?? String.Empty).Split(',')
                                            .Select(x => x.Trim())
                                            .Distinct()
                                            .ToList();
                }
            }
        }

        public override bool ShouldIgnore(IFileSystemInfo info, FileChangeCollectionContext context)
        {
            if (_ignoredPathList == null || _ignoredPathList.Count == 0)
            {
                return false;
            }

            foreach (var path in _ignoredPathList)
            {
                if (path.StartsWith("."))
                {
                    if (info.Extension == path)
                    {
                        return true;
                    }
                }
                else if (path.StartsWith("/"))
                {
                    if (path.TrimEnd('/').Equals(info.VirtualPath, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                else if (path.Equals(info.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
