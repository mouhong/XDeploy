using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XDeploy.Data;

namespace XDeploy
{
    public class WorkContext : IDisposable
    {
        public DeploymentProject Project { get; private set; }

        public Database Database { get; private set; }

        public WorkContext(DeploymentProject project)
        {
            Require.NotNull(project, "project");

            Project = project;
            Database = new Database(Path.Combine(project.ProjectDirectory, "Data\\Data.sqlite"));
        }

        public void Dispose()
        {
            Database.Dispose();
        }

        public static WorkContext _current;

        public static WorkContext Current
        {
            get
            {
                return _current;
            }
        }

        public static void SetCurrent(WorkContext workContext)
        {
            _current = workContext;
        }
    }
}
