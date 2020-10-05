using NHibernate.Cfg;
using TauCode.Db.SQLite;

namespace TauCode.WebApi.Server.Cqrs.Tests.AppHost
{
    public class SQLiteTestConfigurationBuilder
    {
        public SQLiteTestConfigurationBuilder()
        {
            var tuple = SQLiteTools.CreateSQLiteDatabase();
            this.TempDbFilePath = tuple.Item1;
            this.ConnectionString = tuple.Item2;

            var configuration = new Configuration();
            configuration.Properties.Add("connection.connection_string", this.ConnectionString);
            configuration.Properties.Add("connection.driver_class", "NHibernate.Driver.SQLite20Driver");
            configuration.Properties.Add("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
            configuration.Properties.Add("dialect", "NHibernate.Dialect.SQLiteDialect");

            this.Configuration = configuration;
        }

        public string TempDbFilePath { get; }
        public string ConnectionString { get; }
        public Configuration Configuration { get; }
    }
}
