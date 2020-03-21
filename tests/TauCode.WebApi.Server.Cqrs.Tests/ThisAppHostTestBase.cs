using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using TauCode.Infrastructure.Time;

namespace TauCode.WebApi.Server.Cqrs.Tests
{
    [TestFixture]
    public abstract class ThisAppHostTestBase
    {
        protected Dictionary<string, Currency> CurrenciesByCode;
        protected Dictionary<CurrencyId, Currency> CurrenciesById;

        [OneTimeSetUp]
        public void OneTimeSetUpBase()
        {
            this.OneTimeSetUpBaseImpl();
        }

        protected virtual void OneTimeSetUpBaseImpl()
        {
            this.ScriptBuilder.CurrentOpeningIdentifierDelimiter = '[';
            this.Cruder.ScriptBuilder.CurrentOpeningIdentifierDelimiter = '[';
        }

        [SetUp]
        public void SetUpBase()
        {
            this.SetUpBaseImpl();

            TimeProvider.Override(new DateTime(2019, 3, 27, 19, 10, 20));

            this.CurrenciesByCode = this.SetupSession
                .Query<Currency>()
                .ToList()
                .ToDictionary(x => x.Code, x => x);

            this.CurrenciesById = CurrenciesByCode.ToDictionary(x => x.Value.Id, x => x.Value);
        }

        protected virtual void SetUpBaseImpl()
        {
            this.PurgeDb();
            this.Migrate();

            var json = this.GetType().Assembly.GetResourceText("testdb.json", true);
            this.DeserializeDbJson(json);
        }

        protected override HttpClient CreateClient()
        {
            return this.Factory
                .WithWebHostBuilder(builder => builder.UseSolutionRelativeContentRoot(@"tests\IntegrationTests"))
                .CreateClient();
        }

        protected override TestServer GetTestServer()
        {
            return this.Factory.Factories.Single().Server;
        }

        protected override Assembly GetMigrationAssembly()
        {
            return typeof(M0_Baseline).Assembly;
        }

        protected override TargetDbType GetTargetDbType()
        {
            return TargetDbType.SqlServer;
        }

        protected override string GetConnectionString()
        {
            return IntegrationTestHelper.DefaultConnectionString;
        }

        protected override IDbConnection CreateDbConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        protected override WebApplicationFactory<TestStartup> CreateFactory()
        {
            return new TestFactory();
        }
    }
}
