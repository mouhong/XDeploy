using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public class DeploymentTarget
    {
        public static readonly string DefaultBackupFolderNameTemplate = "Before-Deploy-{ReleaseName}";

        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual Location DeployLocation { get; set; }

        public virtual Location BackupRootLocation { get; set; }

        public virtual string BackupFolderNameTemplate { get; set; }

        public virtual DateTime CreatedAtUtc { get; protected set; }

        public virtual DateTime? LastDeployedAtUtc { get; set; }

        public virtual DateTime? LastBackuppedAtUtc { get; set; }

        public DeploymentTarget()
        {
            CreatedAtUtc = DateTime.UtcNow;
            DeployLocation = new Location();
            BackupRootLocation = new Location();
            BackupFolderNameTemplate = DefaultBackupFolderNameTemplate;
        }
        
        public override string ToString()
        {
            return Name;
        }
    }
}
