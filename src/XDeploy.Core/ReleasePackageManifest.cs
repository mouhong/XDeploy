using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XDeploy
{
    [XmlRoot("Manifest")]
    public class ReleasePackageManifest
    {
        public string ReleaseNotes { get; set; }

        public DateTime CreatedTimeUtc { get; set; }

        public List<ReleasePackageDeployLog> DeployLogs { get; set; }

        public ReleasePackageManifest()
        {
            CreatedTimeUtc = DateTime.UtcNow;
            DeployLogs = new List<ReleasePackageDeployLog>();
        }

        public static ReleasePackageManifest LoadFrom(string path)
        {
            using (var reader = new StreamReader(path, true))
            {
                return LoadFrom(reader);
            }
        }

        public static ReleasePackageManifest LoadFrom(TextReader reader)
        {
            var serializer = new XmlSerializer(typeof(ReleasePackageManifest));
            var data = (ReleasePackageManifest)serializer.Deserialize(reader);
            return data;
        }

        public void Save(string path)
        {
            using (var writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                Save(writer);
            }
        }

        public void Save(TextWriter writer)
        {
            var serializer = new XmlSerializer(typeof(ReleasePackageManifest));
            serializer.Serialize(writer, this);
        }
    }

    public class ReleasePackageDeployLog
    {
        public string DeployTargetName { get; set; }

        public DateTime DeployTimeUtc { get; set; }

        public ReleasePackageDeployLog()
        {
            DeployTimeUtc = DateTime.UtcNow;
        }
    }
}
