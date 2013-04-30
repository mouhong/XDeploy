using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public static class Paths
    {
        public static string Release(string projectDirectory, string releaseName)
        {
            return Path.Combine(projectDirectory, "Releases", releaseName);
        }
    }
}
