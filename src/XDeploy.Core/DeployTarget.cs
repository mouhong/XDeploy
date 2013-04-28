using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public class DeployTarget
    {
        public string Name { get; set; }

        public Location DeployLocation { get; set; }

        public Location BackupLocation { get; set; }

        public DateTime? LastDeployTimeUtc { get; set; }

        public DateTime? LastBackupTimeUtc { get; set; }

        public DeployTarget()
        {
        }

        public DeployTarget(string name, Location deployLocation)
        {
            Name = name;
            DeployLocation = deployLocation;
        }
    }
}
