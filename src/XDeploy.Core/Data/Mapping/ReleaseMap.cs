using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDeploy.Data.Mapping.UserTypes;

namespace XDeploy.Data.Mapping
{
    class ReleaseMap : ClassMapping<Release>
    {
        public ReleaseMap()
        {
            this.HighLowId(c => c.Id);

            Property(c => c.Name);
            Property(c => c.ReleaseNotes);
            Property(c => c.CreatedAtUtc);
            Property(c => c.LastDeployedAtUtc);
            Property(c => c.DeploymentInfos, m => m.Type<Blobbed<IList<ReleaseDeploymentInfo>>>());
        }
    }
}
