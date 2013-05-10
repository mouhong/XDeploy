using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDeploy.IO;

namespace XDeploy.Changes
{
    public class FileChange
    {
        public IFile File { get; private set; }

        public FileChecksum OldChecksum { get; private set; }

        public FileChecksum NewChecksum { get; private set; }

        public bool IsNewFile
        {
            get
            {
                return OldChecksum == null;
            }
        }

        public FileChange(IFile file, FileChecksum oldChecksum, FileChecksum newChecksum)
        {
            Require.NotNull(file, "file");
            Require.NotNull(newChecksum, "newChecksum");

            File = file;
            OldChecksum = oldChecksum;
            NewChecksum = newChecksum;
        }
    }
}
