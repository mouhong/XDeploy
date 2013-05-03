using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
                return GetConnectionString(DbFilePath);
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

        public static string GetConnectionString(string dbFilePath)
        {
            return "Data Source=" + dbFilePath + ";Version=3;";
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

                InitializeDatabase(DbFilePath);

                var config = LoadConfiguration();
                _sessionFactory = config.BuildSessionFactory();

                IsInitialized = true;
            }
        }

        private Configuration LoadConfiguration()
        {
            var config = new Configuration();

            config.DataBaseIntegration(x =>
            {
                x.Driver<NHibernate.Driver.SQLite20Driver>();
                x.Dialect<NHibernate.Dialect.SQLiteDialect>();
                x.ConnectionString = ConnectionString;
            });

            config.AddMapping(LoadMapping());

            return config;
        }

        private HbmMapping LoadMapping()
        {
            var mapper = new ModelMapper();

            foreach (var type in typeof(Database).Assembly.GetTypes())
            {
                if (!type.IsClass || type.IsAbstract || type.IsInterface || !type.BaseType.IsGenericType) continue;

                if (type.Namespace == "XDeploy.Data.Mapping" && type.Name.EndsWith("Map"))
                {
                    mapper.AddMapping(type);
                }
            }

            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
            mapping.autoimport = false;

            return mapping;
        }

        public static void InitializeDatabase(string dbFilePath)
        {
            if (File.Exists(dbFilePath))
            {
                return;
            }

            var directory = Path.GetDirectoryName(dbFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            SQLiteConnection.CreateFile(dbFilePath);

            using (var stream = new StreamReader(typeof(Database).Assembly.GetManifestResourceStream("XDeploy.Data.tables.sql")))
            {
                var sql = stream.ReadToEnd();

                using (var conn = new SQLiteConnection(GetConnectionString(dbFilePath)))
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
