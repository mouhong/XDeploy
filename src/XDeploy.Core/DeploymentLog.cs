using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public class DeploymentLog
    {
        public virtual int Id { get; set; }

        public virtual int ReleaseId { get; set; }

        public virtual int DeployTargetId { get; set; }

        public virtual string DeployTargetName { get; set; }

        public virtual bool Success { get; set; }

        public virtual string Message { get; set; }

        public virtual DateTime StartedAtUtc { get; set; }

        public virtual DateTime CompletedAtUtc { get; set; }

        public virtual DateTime? DeploymentStartedAtUtc { get; set; }

        public virtual DateTime? DeploymentCompletedAtUtc { get; set; }

        public virtual DateTime? BackupStartedAtUtc { get; set; }

        public virtual DateTime? BackupCompletedAtUtc { get; set; }

        public DeploymentLog()
        {
        }
    }
}
