using NHibernate;
using NHibernate.Cfg;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using XDeploy.Data.Mapping;

namespace XDeploy.Data
{
    public class Database : IDisposable
    {
        private ISessionFactory _sessionFactory;
        private readonly object _lock = new object();

        public string DbFilePath { get; private set; }

        public bool IsInitialized { get; private set; }

        public string ConnectionString
        {
            get
            {
                return "Data Source=" + DbFilePath + ";Version=3;";
            }
        }

        public Database(string dbFilePath)
        {
            Require.NotNullOrEmpty(dbFilePath, "dbFilePath");
            DbFilePath = dbFilePath;
        }

        public ISession OpenSession()
        {
            EnsureInitialized();
            return _sessionFactory.OpenSession();
        }

        public IStatelessSession OpenStatelessSession()
        {
            EnsureInitialized();
            return _sessionFactory.OpenStatelessSession();
        }

        public void Initialize()
        {
            EnsureInitialized();
        }

        public void Dispose()
        {
            if (_sessionFactory != null)
            {
                _sessionFactory.Dispose();
                _sessionFactory = null;
            }
        }

        private void EnsureInitialized()
        {
            if (IsInitialized)
            {
                return;
            }

            lock (_lock)
            {
                if (IsInitialized)
                {
                    return;
                }

                EnsureDatabaseCreated();

                var config = new Configuration();
                config.DataBaseIntegration(x =>
                {
                    x.Driver<NHibernate.Driver.SQLite20Driver>();
                    x.Dialect<NHibernate.Dialect.SQLiteDialect>();
                    x.ConnectionString = ConnectionString;
                });

                config.AddMapping(ByCodeMappingLoader.LoadMappingFrom(typeof(Database).Assembly));

                _sessionFactory = config.BuildSessionFactory();

                IsInitialized = true;
            }
        }

        private void EnsureDatabaseCreated()
        {
            if (File.Exists(DbFilePath))
            {
                return;
            }

            var directory = Path.GetDirectoryName(DbFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            SQLiteConnection.CreateFile(DbFilePath);

            using (var stream = new StreamReader(typeof(Database).Assembly.GetManifestResourceStream("XDeploy.Data.tables.sql")))
            {
                var sql = stream.ReadToEnd();

                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
