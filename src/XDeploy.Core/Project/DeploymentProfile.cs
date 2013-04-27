using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XDeploy.Project
{
    public class DeploymentProfile
    {
        public string Name { get; set; }

        public Location DeployLocation { get; set; }

        public Location BackupLocation { get; set; }

        public DateTime? LastDeployTimeUtc { get; set; }

        public DeploymentProfile()
        {
        }

        public DeploymentProfile(string name)
        {
            Require.NotNullOrEmpty(name, "name");
            Name = name;
        }
    }
}
