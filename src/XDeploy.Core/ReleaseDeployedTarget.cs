using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public class ReleaseDeployedTarget
    {
        public string TargetId { get; set; }

        public string TargetName { get; set; }

        public DateTime DeployedTimeUtc { get; set; }
    }
}
