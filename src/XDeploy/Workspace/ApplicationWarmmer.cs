using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;

namespace XDeploy.Workspace
{
    static class ApplicationWarmmer
    {
        public static void Warm(WorkContext context)
        {
            context.Database.Initialize();

            using (var session = context.Database.OpenSession())
            {
                // Issue an arbitrary query to warm up queries
                session.Query<DeploymentTarget>().OrderBy(x => x.Id).ToList();
            }
        }
    }
}
