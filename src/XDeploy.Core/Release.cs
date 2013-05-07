using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public class Release
    {
        private List<ReleaseDeploymentInfo> _deploymentInfos = new List<ReleaseDeploymentInfo>();

        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string ReleaseNotes { get; set; }

        public virtual DateTime CreatedAtUtc { get; protected set; }

        public virtual DateTime? LastDeployedAtUtc { get; set; }

        public virtual IEnumerable<ReleaseDeploymentInfo> DeploymentInfos
        {
            get
            {
                return _deploymentInfos;
            }
            protected set
            {
                Require.NotNull(value, "value");
                _deploymentInfos = new List<ReleaseDeploymentInfo>(value);
            }
        }

        public Release()
        {
            CreatedAtUtc = DateTime.UtcNow;
        }

        public virtual ReleaseDeploymentInfo FindDeploymentInfo(int targetId)
        {
            return _deploymentInfos.FirstOrDefault(x => x.TargetId == targetId);
        }

        public virtual void AddDeploymentInfo(ReleaseDeploymentInfo info)
        {
            Require.NotNull(info, "info");
            RemoveDeploymentInfo(info.TargetId);
            _deploymentInfos.Add(info);
        }

        public virtual void RemoveDeploymentInfo(int targetId)
        {
            var info = _deploymentInfos.FirstOrDefault(x => x.TargetId == targetId);
            if (info != null)
            {
                _deploymentInfos.Remove(info);
            }
        }
    }
}
