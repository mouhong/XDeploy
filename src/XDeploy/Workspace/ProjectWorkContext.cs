using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDeploy.Data;

namespace XDeploy.Workspace
{
    public class ProjectWorkContext : IDisposable
    {
        public WorkContext WorkContext { get; private set; }

        public Database Database
        {
            get
            {
                return WorkContext.Database;
            }
        }

        public DeploymentProjectViewModel Project { get; private set; }

        public string ProjectDirectory
        {
            get
            {
                return WorkContext.Project.ProjectDirectory;
            }
        }

        public ProjectWorkContext(DeploymentProject project)
        {
            WorkContext = new WorkContext(project);
            Project = new DeploymentProjectViewModel(project);
        }

        public ISession OpenSession()
        {
            return Database.OpenSession();
        }

        public void Dispose()
        {
            WorkContext.Dispose();
        }

        public void Warmup()
        {
            ApplicationWarmmer.Warm(WorkContext);
        }

        public void WarmupAsync()
        {
            Task.Factory.StartNew(() => Warmup());
        }

        public static ProjectWorkContext Current { get; private set; }

        static readonly object _lock = new object();

        public static void SetCurrent(ProjectWorkContext context)
        {
            lock (_lock)
            {
                if (Current != null)
                {
                    Current.Dispose();
                }
                Current = context;
            }
        }
    }
}
