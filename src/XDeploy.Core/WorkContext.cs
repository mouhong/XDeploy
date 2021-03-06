﻿using NHibernate;
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
            Database = new Database(Paths.DbFile(project.ProjectDirectory));
        }

        public ISession OpenSession()
        {
            return Database.OpenSession();
        }

        public IStatelessSession OpenStatelessSession()
        {
            return Database.OpenStatelessSession();
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
