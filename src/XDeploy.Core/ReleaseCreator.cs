using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XDeploy.Data;
using NHibernate.Linq;

namespace XDeploy
{
    public class ReleaseCreator
    {
        private WorkContext _workContext;

        public ReleaseCreator(WorkContext context)
        {
            _workContext = context;
        }

        public void CreateRelease(string name, string releaseNotes, DeploymentSettings deploymentSettings)
        {
            Require.NotNullOrEmpty(name, "name");

            var project = _workContext.Project;

            using (var session = _workContext.Database.OpenSession())
            {
                var lastRelease = session.Query<Release>().OrderByDescending(x => x.Id).FirstOrDefault();

                if (deploymentSettings == null)
                {
                    deploymentSettings = new DeploymentSettings
                    {
                        IgnorantRules = project.IgnorantRules,
                        DeployItemsModifiedSinceUtc = lastRelease == null ? null : (DateTime?)lastRelease.CreatedAtUtc
                    };
                }

                var builder = new ReleasePackageBuilder();
                builder.Build(project.SourceDirectory, Paths.Release(project.ProjectDirectory, name), deploymentSettings);

                var release = new Release
                {
                    Name = name,
                    ReleaseNotes = releaseNotes
                };

                session.Save(release);
                session.Commit();

                project.LastReleaseCreatedAtUtc = release.CreatedAtUtc;
                project.TotalReleases++;

                project.Save();
            }
        }
    }
}
