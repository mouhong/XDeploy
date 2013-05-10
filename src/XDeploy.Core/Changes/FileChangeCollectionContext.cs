using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDeploy.IO;

namespace XDeploy.Changes
{
    public class FileChangeCollectionContext
    {
        public IDirectory SourceDirectory { get; private set; }

        public IEnumerable<AbstractIgnorantRule> IgnorantRules { get; private set; }

        public IEnumerable<FileChecksum> CurrentFileChecksums { get; private set; }

        public FileChangeCollectionContext(
            IDirectory sourceDirectory,
            IEnumerable<FileChecksum> currentFileChecksums,
            IEnumerable<AbstractIgnorantRule> ignorantRules)
        {
            Require.NotNull(sourceDirectory, "sourceDirectory");

            SourceDirectory = sourceDirectory;
            CurrentFileChecksums = currentFileChecksums ?? Enumerable.Empty<FileChecksum>();
            IgnorantRules = ignorantRules ?? Enumerable.Empty<AbstractIgnorantRule>();
        }

        public FileChecksum GetCurrentFileChecksum(string fileVirtualPath)
        {
            Require.NotNullOrEmpty(fileVirtualPath, "fileVirtualPath");
            return CurrentFileChecksums.FirstOrDefault(x => x.VirtualPath.Equals(fileVirtualPath, StringComparison.OrdinalIgnoreCase));
        }
    }
}
