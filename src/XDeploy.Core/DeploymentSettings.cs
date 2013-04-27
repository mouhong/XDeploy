using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public class DeploymentSettings
    {
        public bool DeleteExistingFiles { get; set; }

        public List<IIgnorantRule> IgnorantRules { get; set; }

        public DeploymentSettings()
        {
            IgnorantRules = new List<IIgnorantRule>();
        }
    }
}
