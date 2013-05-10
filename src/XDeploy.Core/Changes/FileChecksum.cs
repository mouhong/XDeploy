using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.Changes
{
    public class FileChecksum
    {
        public virtual string VirtualPath { get; protected set; }

        public virtual string Checksum { get; set; }

        public virtual DateTime LastUpdatedAtUtc { get; set; }

        protected FileChecksum()
        {
        }

        public FileChecksum(string virtualPath)
        {
            Require.NotNullOrEmpty(virtualPath, "virtualPath");
            VirtualPath = virtualPath;
            LastUpdatedAtUtc = DateTime.UtcNow;
        }

        public FileChecksum(string virtualPath, string checksum)
            : this(virtualPath)
        {
            Require.NotNullOrEmpty(checksum, "checksum");
            Checksum = checksum;
        }

        public virtual void UpdateFrom(FileChecksum state)
        {
            Require.NotNull(state, "state");
            Checksum = state.Checksum;
            LastUpdatedAtUtc = DateTime.UtcNow;
        }
    }
}
