using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public class Release
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string ReleaseNotes { get; set; }

        public virtual DateTime CreatedAtUtc { get; protected set; }

        public virtual DateTime? LastDeployedAtUtc { get; set; }

        public virtual IList<ReleaseDeployedTarget> DeployedTargets { get; protected set; }

        public Release()
        {
            CreatedAtUtc = DateTime.UtcNow;
            DeployedTargets = new List<ReleaseDeployedTarget>();
        }
    }
}
