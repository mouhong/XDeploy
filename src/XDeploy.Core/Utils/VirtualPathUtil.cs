using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace XDeploy.Utils
{
    public static class VirtualPathUtil
    {
        public static string Combine(params string[] paths)
        {
            string path = String.Empty;

            IEnumerator enumerator = paths.GetEnumerator();
            if (enumerator.MoveNext())
            {
                path = (String)enumerator.Current;
            }

            while (enumerator.MoveNext())
            {
                path = Combine(path, (String)enumerator.Current);
            }

            return path;
        }

        public static string Combine(string path1, string path2)
        {
            if (String.IsNullOrEmpty(path1))
            {
                return path2;
            }
            if (String.IsNullOrEmpty(path2))
            {
                return path1;
            }

            string result = null;

            if (!path1.EndsWith("/"))
            {
                path1 = path1 + "/";
            }
            if (path2.StartsWith("/"))
            {
                if (path2.Length == 1)
                {
                    result = path1;
                }
                else
                {
                    result = String.Concat(path1, path2.Substring(1));
                }
            }
            else
            {
                result = String.Concat(path1, path2);
            }

            return result;
        }

        public static string GetDirectory(string virtualPath)
        {
            virtualPath = virtualPath.TrimEnd('/');

            if (String.IsNullOrEmpty(virtualPath))
            {
                return null;
            }

            var index = virtualPath.LastIndexOf('/');

            if (index < 0)
            {
                return null;
            }

            if (index == 0)
            {
                return "/";
            }

            return virtualPath.Substring(0, index);
        }

        public static string GetVirtualPath(FileSystemInfo file, DirectoryInfo rootDirectory)
        {
            var fileVirtualPath = file.FullName.Substring(rootDirectory.FullName.Length).Replace('\\', '/');
            if (!fileVirtualPath.StartsWith("/"))
            {
                fileVirtualPath = "/" + fileVirtualPath;
            }

            return fileVirtualPath;
        }
    }
}
