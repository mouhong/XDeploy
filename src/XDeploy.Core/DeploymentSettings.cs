using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public class DeploymentSettings
    {
        public DateTime? DeployItemsModifiedSinceUtc { get; set; }

        public List<AbstractIgnorantRule> IgnorantRules { get; set; }

        public DeploymentSettings()
        {
            IgnorantRules = new List<AbstractIgnorantRule>();
        }
    }
}
