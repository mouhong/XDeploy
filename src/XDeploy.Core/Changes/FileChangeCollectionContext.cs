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

        public IEnumerable<FileChecksum> CurrentFileStates { get; private set; }

        public FileChangeCollectionContext(
            IDirectory sourceDirectory,
            IEnumerable<FileChecksum> currentFileStates,
            IEnumerable<AbstractIgnorantRule> ignorantRules)
        {
            SourceDirectory = sourceDirectory;
            CurrentFileStates = currentFileStates;
            IgnorantRules = ignorantRules;
        }

        public FileChecksum GetCurrentFileChecksum(string fileVirtualPath)
        {
            Require.NotNullOrEmpty(fileVirtualPath, "fileVirtualPath");
            return CurrentFileStates.FirstOrDefault(x => x.VirtualPath.Equals(fileVirtualPath, StringComparison.OrdinalIgnoreCase));
        }
    }
}
