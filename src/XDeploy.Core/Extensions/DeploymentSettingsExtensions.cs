using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public static class DeploymentSettingsExtensions
    {
        public static DeploymentSettings Ignore(this DeploymentSettings settings, params string[] paths)
        {
            Require.NotNull(settings, "settings");
            Require.NotNull(paths, "paths");

            var rule = settings.IgnorantRules.FirstOrDefault(x => x is PathIgnorantRule) as PathIgnorantRule;
            if (rule == null)
            {
                rule = new PathIgnorantRule();
                rule.IgnorePaths(paths);
                settings.IgnorantRules.Add(rule);
            }
            else
            {
                rule.IgnorePaths(paths);
            }

            return settings;
        }
    }
}
