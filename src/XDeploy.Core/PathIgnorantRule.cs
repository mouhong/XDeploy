using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public class PathIgnorantRule : IIgnorantRule
    {
        private List<string> _ignoredPaths = new List<string>();

        public IEnumerable<string> IgnoredPaths
        {
            get
            {
                return _ignoredPaths;
            }
        }

        public PathIgnorantRule()
        {
        }

        public PathIgnorantRule(IEnumerable<string> ignoredPaths)
        {
            Require.NotNull(ignoredPaths, "ignoredPaths");
            _ignoredPaths = new List<string>(ignoredPaths);
        }

        public void IgnorePaths(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                IgnorePath(path);
            }
        }

        public void IgnorePath(string path)
        {
            Require.NotNullOrEmpty(path, "path");

            if (!_ignoredPaths.Any(x => x.Equals(path, StringComparison.OrdinalIgnoreCase)))
            {
                _ignoredPaths.Add(path);
            }
        }

        public bool ShouldIgnore(FileSystemInfo info, DirectoryInfo rootDirectory)
        {
            foreach (var path in IgnoredPaths)
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
                    var filePath = info.FullName.Substring(rootDirectory.FullName.Length).Replace('\\', '/');
                    if (!filePath.StartsWith("/"))
                    {
                        filePath = "/" + filePath;
                    }

                    filePath.TrimEnd('/');

                    if (path.TrimEnd('/').Equals(filePath, StringComparison.OrdinalIgnoreCase))
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
