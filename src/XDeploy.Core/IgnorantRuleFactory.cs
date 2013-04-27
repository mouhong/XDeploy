using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy
{
    static class IgnorantRuleFactory
    {
        static readonly List<Type> _allIgnorantRuleTypes = new List<Type>();

        public static IEnumerable<Type> AllIgnorantRuleTypes
        {
            get
            {
                return _allIgnorantRuleTypes;
            }
        }

        static IgnorantRuleFactory()
        {
            var baseType = typeof(AbstractIgnorantRule);

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in asm.GetExportedTypes())
                {
                    if (type.IsClass && !type.IsAbstract && baseType.IsAssignableFrom(type))
                    {
                        _allIgnorantRuleTypes.Add(type);
                    }
                }
            }
        }
    }
}
