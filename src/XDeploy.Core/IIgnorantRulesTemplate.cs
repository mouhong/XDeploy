using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public interface IIgnorantRulesTemplate
    {
        IList<AbstractIgnorantRule> GetDefaultIgnorantRules();
    }

    public class AspNetApplicationIgnorantRulesTemplate : IIgnorantRulesTemplate
    {
        public IList<AbstractIgnorantRule> GetDefaultIgnorantRules()
        {
            return new List<AbstractIgnorantRule>
            {
                new PathIgnorantRule(new[] { "obj", ".user", ".csproj", ".cs", ".suo" })
            };
        }
    }

    public class AspNetWebsiteIgnorantRulesTemplate : IIgnorantRulesTemplate
    {
        public IList<AbstractIgnorantRule> GetDefaultIgnorantRules()
        {
            return new List<AbstractIgnorantRule>
            {
                new PathIgnorantRule(new[] { "obj", ".user", ".csproj", ".suo" })
            };
        }
    }
}
