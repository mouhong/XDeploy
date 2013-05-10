using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XDeploy.Data;
using NHibernate.Linq;
using XDeploy.IO;
using XDeploy.Changes;

namespace XDeploy
{
    public class ReleaseBuilder
    {
        private WorkContext _workContext;

        public ReleaseBuilder(WorkContext context)
        {
            Require.NotNull(context, "context");
            _workContext = context;
        }

        public void BuildRelease(string name, string releaseNotes, IEnumerable<AbstractIgnorantRule> ignorantRules = null)
        {
            Require.NotNullOrEmpty(name, "name");

            var project = _workContext.Project;

            if (ignorantRules == null)
            {
                ignorantRules = project.IgnorantRules;
            }

            using (var session = _workContext.Database.OpenSession())
            {
                var currentFileStates = session.Query<FileChecksum>().ToList();

                var sourceDirectory = Directories.GetDirectory(project.SourceDirectory);
                var releaseDirectory = Directories.GetDirectory(Paths.ReleaseFiles(project.ProjectDirectory, name));

                // Collect changed files and copy them to release directory
                var collector = new FileChangeCollector();
                var context = new FileChangeCollectionContext(sourceDirectory, currentFileStates, ignorantRules);
                var changes = collector.Collect(context);

                foreach (var change in changes)
                {
                    CopyChangedFileToReleaseDirectory(change.File, releaseDirectory);

                    if (change.IsNewFile)
                    {
                        session.Save(change.NewChecksum);
                    }
                    else
                    {
                        change.OldChecksum.UpdateFrom(change.NewChecksum);
                    }
                }

                // Create release in database
                var release = new Release
                {
                    Name = name,
                    ReleaseNotes = releaseNotes
                };

                session.Save(release);
                session.Commit();

                // Update project file
                project.LastReleaseCreatedAtUtc = release.CreatedAtUtc;
                project.TotalReleases++;

                project.Save();
            }
        }

        private void CopyChangedFileToReleaseDirectory(IFile newFile, IDirectory releaseDirectory)
        {
            var toFile = releaseDirectory.GetFile(newFile.VirtualPath);
            FileOverwritter.Overwrite(toFile, newFile);
        }
    }
}
