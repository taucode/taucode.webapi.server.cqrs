using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using TauCode.Db;
using TauCode.Db.Data;
using TauCode.Db.FluentMigrations;
using TauCode.Db.Testing;
using TauCode.Extensions;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.DbMigrations;

namespace TauCode.WebApi.Server.Cqrs.Tests
{
    public class DbTodoTest : DbTestBase
    {
        private class CurrencyDto
        {
            public string Id { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
        }

        private string _connectionString;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.OneTimeSetUpImpl();
        }

        protected override void OneTimeSetUpImpl()
        {
            var tuple = DbUtils.CreateSQLiteConnectionString();
            _connectionString = tuple.Item2;

            base.OneTimeSetUpImpl();

            var migrator = new FluentDbMigrator(
                this.GetDbProviderName(),
                this.GetConnectionString(),
                typeof(M0_Baseline).Assembly);

            migrator.Migrate();
        }

        protected override string GetDbProviderName() => DbProviderNames.SQLite;

        protected override string GetConnectionString() => _connectionString;

        [Test]
        public void Wat()
        {
            var json = this.GetType().Assembly.GetResourceText(".currencies.json", true);
            var currencies = JsonConvert.DeserializeObject<IList<CurrencyDto>>(json);

            var currencyRows = currencies
                .Select(x => new DynamicRow(new
                {
                    id = x.Id,
                    code = x.Code,
                    name = x.Name,
                }))
                .ToList();

            this.Cruder.InsertRows("currency", currencyRows);

            var dbJson = this.DbSerializer.SerializeDbData();
            File.WriteAllText(@"c:\temp\curo.json", dbJson, Encoding.UTF8);
        }
    }
}
