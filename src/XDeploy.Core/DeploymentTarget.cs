using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public class DeploymentTarget
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual Location DeployLocation { get; set; }

        public virtual Location BackupLocation { get; set; }

        public virtual DateTime? LastDeployedAtUtc { get; set; }

        public virtual DateTime? LastBackuppedAtUtc { get; set; }

        public DeploymentTarget()
        {
            DeployLocation = new Location();
            BackupLocation = new Location();
        }

        public DeploymentTarget(string name, Location deployLocation)
        {
            Name = name;
            DeployLocation = deployLocation;
            BackupLocation = new Location();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
