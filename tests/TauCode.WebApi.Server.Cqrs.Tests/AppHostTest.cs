using NHibernate;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Net.Http;
using TauCode.Db;
using TauCode.Db.FluentMigrations;
using TauCode.Db.Testing;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.DbMigrations;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost.Domain.Currencies;

namespace TauCode.WebApi.Server.Cqrs.Tests
{
    [TestFixture]
    public abstract class AppHostTest : DbTestBase
    {
        protected Dictionary<string, Currency> CurrenciesByCode;
        protected Dictionary<CurrencyId, Currency> CurrenciesById;

        protected HttpClient HttpClient => throw new NotImplementedException();

        protected ISession SetupSession => throw new NotImplementedException();
        protected ISession AssertSession => throw new NotImplementedException();

        [OneTimeSetUp]
        public void OneTimeSetUpBase()
        {
            this.OneTimeSetUpImpl();
        }

        protected override void OneTimeSetUpImpl()
        {
            base.OneTimeSetUpImpl();
            ((SQLiteConnection) this.Connection).BoostSQLiteInsertions();

            var migrator = new FluentDbMigrator(
                this.GetDbProviderName(),
                this.GetConnectionString(),
                typeof(M0_Baseline).Assembly);

            migrator.Migrate();
        }

        [SetUp]
        public void SetUpBase()
        {
            throw new NotImplementedException();
            //this.SetUpBaseImpl();

            //TimeProvider.Override(new DateTime(2019, 3, 27, 19, 10, 20));

            //this.CurrenciesByCode = this.SetupSession
            //    .Query<Currency>()
            //    .ToList()
            //    .ToDictionary(x => x.Code, x => x);

            //this.CurrenciesById = CurrenciesByCode.ToDictionary(x => x.Value.Id, x => x.Value);
        }

        protected virtual void SetUpBaseImpl()
        {
            throw new NotImplementedException();

            //this.PurgeDb();
            //this.Migrate();

            //var json = this.GetType().Assembly.GetResourceText("testdb.json", true);
            //this.DeserializeDbJson(json);
        }

        protected override string GetConnectionString()
        {
            throw new NotImplementedException();
        }

        protected override string GetDbProviderName() => DbProviderNames.SQLite;

        //protected override HttpClient CreateClient()
        //{
        //    return this.Factory
        //        .WithWebHostBuilder(builder => builder.UseSolutionRelativeContentRoot(@"tests\IntegrationTests"))
        //        .CreateClient();
        //}

        //protected override TestServer GetTestServer()
        //{
        //    return this.Factory.Factories.Single().Server;
        //}

        //protected override Assembly GetMigrationAssembly()
        //{
        //    return typeof(M0_Baseline).Assembly;
        //}

        //protected override TargetDbType GetTargetDbType()
        //{
        //    return TargetDbType.SqlServer;
        //}

        //protected override string GetConnectionString()
        //{
        //    return IntegrationTestHelper.DefaultConnectionString;
        //}

        //protected override IDbConnection CreateDbConnection(string connectionString)
        //{
        //    return new SqlConnection(connectionString);
        //}

        //protected override WebApplicationFactory<TestStartup> CreateFactory()
        //{
        //    return new TestFactory();
        //}
    }
}
