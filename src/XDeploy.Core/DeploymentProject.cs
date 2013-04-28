using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using XDeploy.Storage;

namespace XDeploy
{
    public class DeploymentProject
    {
        public string Name { get; set; }

        [XmlIgnore]
        public string Path { get; set; }

        public string ProjectDirectory
        {
            get
            {
                if (String.IsNullOrEmpty(Path))
                {
                    return null;
                }

                return System.IO.Path.GetDirectoryName(Path);
            }
        }

        public string SourceDirectory { get; set; }

        public string ReleasesFolderName { get; set; }

        public Location BackupRootLocation { get; set; }

        public DateTime? LastReleaseCreationTimeUtc { get; set; }

        [XmlArrayItem("IgnorantRule")]
        public List<AbstractIgnorantRule> IgnorantRules { get; set; }

        public DeploymentProject()
        {
            ReleasesFolderName = "Releases";
            IgnorantRules = new List<AbstractIgnorantRule>();
        }

        public ReleasePackage CreateReleasePackage(string name, string releaseNotes)
        {
            return CreateReleasePackage(name, releaseNotes, null);
        }

        public ReleasePackage CreateReleasePackage(string name, string releaseNotes, DeploymentSettings deploymentSettings)
        {
            if (deploymentSettings == null)
            {
                deploymentSettings = new DeploymentSettings
                {
                    IgnorantRules = IgnorantRules,
                    DeployItemsModifiedSinceUtc = LastReleaseCreationTimeUtc
                };
            }

            var packager = new ReleasePackager(SourceDirectory, deploymentSettings);

            var package = packager.CreateReleasePackage(name, releaseNotes, System.IO.Path.Combine(ProjectDirectory, ReleasesFolderName));

            LastReleaseCreationTimeUtc = DateTime.UtcNow;

            Save();

            return package;
        }

        public ReleasePackage LoadReleasePackage(string name)
        {
            return ReleasePackage.LoadFrom(System.IO.Path.Combine(ProjectDirectory, ReleasesFolderName, name));
        }

        // TODO: Need to introduce Deploy Target
        public void DeployReleasePackage(string name, IDirectory deployDirectory, bool backup = true)
        {
            if (backup && BackupRootLocation == null)
                throw new InvalidOperationException("Backup root location is not specified.");

            IDirectory backupDirectory = null;

            if (backup)
            {
                var backupRoot = Directories.GetDirectory(BackupRootLocation.Uri, BackupRootLocation.UserName, BackupRootLocation.Password);
                backupDirectory = backupRoot.GetDirectory("Backup-Before-" + name);
            }

            var package = LoadReleasePackage(name);
            var deployer = new ReleasePackageDeployer();
            deployer.Deploy(package, deployDirectory, backupDirectory);

            //var log = new PackageDeployLog
            //{
            //    DeployLocation = new Location(deployDirectory.Uri, deployDirectory.Credential)
            //};

            //if (backupDirectory != null)
            //{
            //    log.BackupLocation = new Location(backupDirectory.Uri, backupDirectory.Credential);
            //}

            //package.Manifest.DeployLogs.Add(log);
            //package.Manifest.Save(package.ManifestFilePath);
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

        static DeploymentProject LoadFrom(TextReader reader)
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

            var directory = System.IO.Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (BackupRootLocation == null)
            {
                BackupRootLocation = new Location(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), "Backups"));
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
