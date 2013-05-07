using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.Data.Mapping
{
    class DeploymentTargetMap : ClassMapping<DeploymentTarget>
    {
        public DeploymentTargetMap()
        {
            this.HighLowId(c => c.Id);

            Property(c => c.Name);
            Property(c => c.CreatedAtUtc);
            Property(c => c.LastDeployedAtUtc);
            Property(c => c.LastBackuppedAtUtc);
            Property(c => c.BackupFolderNameTemplate);

            Component(c => c.DeployLocation, m =>
            {
                m.Property(x => x.Uri, x => x.Column("DeployLocation_Uri"));
                m.Property(x => x.UserName, x => x.Column("DeployLocation_UserName"));
                m.Property(x => x.Password, x => x.Column("DeployLocation_Password"));
            });
            Component(c => c.BackupRootLocation, m =>
            {
                m.Property(x => x.Uri, x => x.Column("BackupRootLocation_Uri"));
                m.Property(x => x.UserName, x => x.Column("BackupRootLocation_UserName"));
                m.Property(x => x.Password, x => x.Column("BackupRootLocation_Password"));
            });
        }
    }
}
