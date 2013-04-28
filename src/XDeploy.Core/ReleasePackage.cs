using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public class ReleasePackage
    {
        public string Name { get; private set; }

        public string Path { get; private set; }

        public ReleasePackageManifest Manifest { get; private set; }

        public string FilesDirectoryPath
        {
            get
            {
                return System.IO.Path.Combine(Path, "Files");
            }
        }

        public string ManifestFilePath
        {
            get
            {
                return System.IO.Path.Combine(Path, "Manifest.xml");
            }
        }

        public ReleasePackage(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public void Refresh()
        {
            Manifest = ReleasePackageManifest.LoadFrom(ManifestFilePath);
        }

        public static ReleasePackage LoadFrom(string packageDirectory)
        {
            var package = new ReleasePackage(System.IO.Path.GetFileName(packageDirectory), packageDirectory);
            package.Manifest = ReleasePackageManifest.LoadFrom(package.ManifestFilePath);

            return package;
        }
    }
}
