using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XDeploy.IO;

namespace XDeploy.Changes
{
    public class FileChangeCollector
    {
        public IList<FileChange> Collect(FileChangeCollectionContext context)
        {
            Require.NotNull(context, "context");
            return CollectRecursively(context.SourceDirectory, context);
        }

        private IList<FileChange> CollectRecursively(IDirectory directory, FileChangeCollectionContext context)
        {
            var changes = new List<FileChange>();

            foreach (var info in directory.GetFileSystemInfos())
            {
                var ignore = false;

                // Check if this file system info should be ignored
                foreach (var rule in context.IgnorantRules)
                {
                    if (rule.ShouldIgnore(info, context))
                    {
                        ignore = true;
                        break;
                    }
                }

                FileChecksum newChecksum = null;
                FileChecksum oldChecksum = null;

                if (!ignore && info is IFile)
                {
                    var file = info as IFile;

                    // Check if file is changed
                    oldChecksum = context.GetCurrentFileChecksum(file.VirtualPath);
                    newChecksum = new FileChecksum(file.VirtualPath)
                    {
                        Checksum = ChecksumComputer.Compute(file)
                    };

                    // if current file state is null, then this is a new file;
                    // if checksum is same, then this file is not changed;
                    if (oldChecksum != null && oldChecksum.Checksum == newChecksum.Checksum)
                    {
                        ignore = true;
                    }
                }

                if (!ignore)
                {
                    if (info is IFile)
                    {
                        changes.Add(new FileChange((IFile)info, oldChecksum, newChecksum));
                    }
                    else
                    {
                        changes.AddRange(CollectRecursively((IDirectory)info, context));
                    }
                }
            }

            return changes;
        }
    }
}
