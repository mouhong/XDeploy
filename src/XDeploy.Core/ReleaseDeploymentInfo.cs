using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public class ReleaseDeploymentInfo
    {
        public int TargetId { get; set; }

        public string TargetName { get; set; }

        public string DeployLocationUri { get; set; }

        public string BackupLocationUri { get; set; }

        public DateTime DeployedAtUtc { get; set; }

        public ReleaseDeploymentInfo()
        {
            DeployedAtUtc = DateTime.UtcNow;
        }

        public ReleaseDeploymentInfo(int targetId, string targetName)
        {
            TargetId = targetId;
            TargetName = targetName;
        }
    }
}
