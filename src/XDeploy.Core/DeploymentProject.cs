using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using XDeploy.Data;
using XDeploy.IO;

namespace XDeploy
{
    public class DeploymentProject
    {
        [XmlIgnore]
        public string Name { get; set; }

        [XmlIgnore]
        public string Path { get; set; }

        public string SourceDirectory { get; set; }

        [XmlIgnore]
        public string ProjectDirectory
        {
            get
            {
                return System.IO.Path.GetDirectoryName(Path);
            }
        }

        [XmlIgnore]
        public string DbFilePath
        {
            get
            {
                return Paths.DbFile(ProjectDirectory);
            }
        }

        public DateTime? LastReleaseCreatedAtUtc { get; set; }

        public int TotalReleases { get; set; }

        public int TotalDeployTargets { get; set; }

        [XmlArrayItem("IgnorantRule")]
        public List<AbstractIgnorantRule> IgnorantRules { get; set; }

        public DeploymentProject()
        {
            IgnorantRules = new List<AbstractIgnorantRule>();
        }

        public void InitializeDatabase()
        {
            Database.InitializeDatabase(DbFilePath);
        }

        public void Save()
        {
            Save(Path);
        }

        public void Save(string path)
        {
            Require.NotNullOrEmpty(path, "path");

            var directory = System.IO.Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                Save(writer);
            }

            Path = path;
        }

        private void Save(TextWriter writer)
        {
            Require.NotNull(writer, "writer");

            var serializer = new XmlSerializer(
                typeof(DeploymentProject),
                IgnorantRuleFactory.AllIgnorantRuleTypes.ToArray());

            serializer.Serialize(writer, this);
        }
    }
}
