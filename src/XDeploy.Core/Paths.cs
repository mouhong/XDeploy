using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public static class Paths
    {
        public static string DbFile(string projectDirectory)
        {
            Require.NotNullOrEmpty(projectDirectory, "projectDirectory");
            return Path.Combine(projectDirectory, "Data", "Data.sqlite");
        }

        public static string Releases(string projectDirectory)
        {
            Require.NotNullOrEmpty(projectDirectory, "projectDirectory");
            return Path.Combine(projectDirectory, "Releases");
        }

        public static string Release(string projectDirectory, string releaseName)
        {
            Require.NotNullOrEmpty(projectDirectory, "projectDirectory");
            Require.NotNullOrEmpty(releaseName, "releaseName");

            return Path.Combine(Releases(projectDirectory), releaseName);
        }

        public static string ReleaseFiles(string projectDirectory, string releaseName)
        {
            Require.NotNullOrEmpty(projectDirectory, "projectDirectory");
            Require.NotNullOrEmpty(releaseName, "releaseName");
            
            return Path.Combine(Release(projectDirectory, releaseName), "Files");
        }
    }
}
