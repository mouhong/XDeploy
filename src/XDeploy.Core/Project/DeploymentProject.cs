using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using XDeploy.Storage;

namespace XDeploy.Project
{
    public class DeploymentProject
    {
        public string Name { get; set; }

        [XmlIgnore]
        public string Path { get; set; }

        public string SourceDirectory { get; set; }

        [XmlArrayItem("IgnorantRule")]
        public List<AbstractIgnorantRule> IgnorantRules { get; set; }

        public List<DeploymentProfile> Profiles { get; set; }

        public DeploymentProject()
        {
            Profiles = new List<DeploymentProfile>();
            IgnorantRules = new List<AbstractIgnorantRule>();
        }

        public void DeployByProfile(string profileName)
        {
            Require.NotNullOrEmpty(profileName, "profileName");

            var profile = Profiles.FirstOrDefault(x => x.Name == profileName);

            if (profile == null)
                throw new InvalidOperationException("Deployment profile \"" + profileName + "\" was not found.");

            var deployer = new FileDeployer(SourceDirectory, new DeploymentSettings
            {
                DeployItemsModifiedSinceUtc = profile.LastDeployTimeUtc,
                IgnorantRules = IgnorantRules
            });

            if (profile.BackupLocation != null)
            {
                var backupRootLocation = StorageLocation.Create(profile.BackupLocation.Uri, profile.BackupLocation.UserName, profile.BackupLocation.Password);
                deployer.BackupLocation = backupRootLocation.GetLocation(new DefaultBackupFolderNameGenerator().Generate(backupRootLocation));
            }

            deployer.Deploy(StorageLocation.Create(profile.DeployLocation.Uri, profile.DeployLocation.UserName, profile.DeployLocation.Password));

            profile.LastDeployTimeUtc = DateTime.UtcNow;
        }

        public static DeploymentProject LoadFrom(string path)
        {
            Require.NotNull(path, "path");
            Require.That(File.Exists(path), "Cannot find XDeploy project file in: " + path + ".");

            using (var reader = new StreamReader(path, true))
            {
                var project = LoadFrom(reader);
                project.Path = path;

                if (String.IsNullOrEmpty(project.Name))
                {
                    project.Name = System.IO.Path.GetFileNameWithoutExtension(path);
                }

                return project;
            }
        }

        public static DeploymentProject LoadFrom(TextReader reader)
        {
            Require.NotNull(reader, "reader");

            var serializer = new XmlSerializer(
                typeof(DeploymentProject),
                IgnorantRuleFactory.AllIgnorantRuleTypes.ToArray());

            return (DeploymentProject)serializer.Deserialize(reader);
        }

        public void Save()
        {
            if (String.IsNullOrEmpty(Path))
                throw new InvalidOperationException("Path property cannot be null or empty.");

            Save(Path);
        }

        public void Save(string path)
        {
            Require.NotNullOrEmpty(path, "path");

            using (var writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                Save(writer);
            }

            Path = path;
        }

        public void Save(TextWriter writer)
        {
            Require.NotNull(writer, "writer");

            var serializer = new XmlSerializer(
                typeof(DeploymentProject),
                IgnorantRuleFactory.AllIgnorantRuleTypes.ToArray());

            serializer.Serialize(writer, this);
        }
    }
}
