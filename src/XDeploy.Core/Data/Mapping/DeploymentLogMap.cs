using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.Data.Mapping
{
    class DeploymentLogMap : ClassMapping<DeploymentLog>
    {
        public DeploymentLogMap()
        {
            this.HighLowId(c => c.Id);

            Property(c => c.ReleaseId);
            Property(c => c.DeployTargetId);
            Property(c => c.DeployTargetName);
            Property(c => c.Success);
            Property(c => c.Message);
            Property(c => c.StartedAtUtc);
            Property(c => c.CompletedAtUtc);
            Property(c => c.DeploymentStartedAtUtc);
            Property(c => c.DeploymentCompletedAtUtc);
            Property(c => c.BackupStartedAtUtc);
            Property(c => c.BackupCompletedAtUtc);
        }
    }
}
