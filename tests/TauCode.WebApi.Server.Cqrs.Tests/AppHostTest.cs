using Autofac;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using NUnit.Framework;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using TauCode.Db;
using TauCode.Db.Testing;
using TauCode.WebApi.Server.Cqrs.Tests.AppHost;

// todo clean up
namespace TauCode.WebApi.Server.Cqrs.Tests
{
    [TestFixture]
    public abstract class AppHostTest : DbTestBase
    {
        private string _connectionString;
        private string _tempDbFilePath;

        protected TestFactory Factory { get; private set; }
        protected HttpClient HttpClient { get; private set; }
        protected ILifetimeScope Container { get; private set; }

        protected ILifetimeScope SetupLifetimeScope { get; private set; }
        protected ILifetimeScope TestLifetimeScope { get; private set; }
        protected ILifetimeScope AssertLifetimeScope { get; private set; }

        protected ISession SetupSession { get; set; }
        protected ISession TestSession { get; set; }
        protected ISession AssertSession { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUpBase()
        {
            this.OneTimeSetUpImpl();
        }

        [OneTimeTearDown]
        public void OneTimeTearDownBase()
        {
            this.OneTimeTearDownImpl();
        }

        [SetUp]
        public void SetUpBase()
        {
            this.SetUpImpl();
        }

        [TearDown]
        public void TearDownBase()
        {
            this.TearDownImpl();
        }
        
        protected override void OneTimeSetUpImpl()
        {
            Inflector.Inflector.SetDefaultCultureFunc = () => new CultureInfo("en-US");

            this.Factory = new TestFactory();

            this.HttpClient = this.Factory
                .WithWebHostBuilder(builder => builder.UseSolutionRelativeContentRoot(@"tests\TauCode.WebApi.Server.Cqrs.Tests"))
                .CreateClient();

            var testServer = this.Factory.Factories.Single().Server;

            var startup = (Startup)testServer.Services.GetService<IAutofacStartup>();
            this.Container = startup.AutofacContainer;

            _connectionString = startup.SQLiteTestConfigurationBuilder.ConnectionString;
            _tempDbFilePath = startup.SQLiteTestConfigurationBuilder.TempDbFilePath;

            base.OneTimeSetUpImpl();
        }

        protected override void OneTimeTearDownImpl()
        {
            base.OneTimeTearDownImpl();

            this.HttpClient.Dispose();
            this.Factory.Dispose();

            this.HttpClient = null;
            this.Factory = null;

            try
            {
                File.Delete(_tempDbFilePath);
            }
            catch
            {
                // ignore
            }
        }

        protected override void SetUpImpl()
        {
            base.SetUpImpl();

            this.SetupLifetimeScope = this.Container.BeginLifetimeScope();
            this.TestLifetimeScope = this.Container.BeginLifetimeScope();
            this.AssertLifetimeScope = this.Container.BeginLifetimeScope();

            // nhibernate stuff
            this.SetupSession = this.SetupLifetimeScope.Resolve<ISession>();
            this.TestSession = this.TestLifetimeScope.Resolve<ISession>();
            this.AssertSession = this.AssertLifetimeScope.Resolve<ISession>();
        }

        protected override void TearDownImpl()
        {
            base.TearDownImpl();

            this.SetupSession.Dispose();
            this.TestSession.Dispose();
            this.AssertSession.Dispose();

            this.SetupLifetimeScope.Dispose();
            this.TestLifetimeScope.Dispose();
            this.AssertLifetimeScope.Dispose();
        }

        protected override string GetConnectionString() => _connectionString;

        protected override string GetDbProviderName() => DbProviderNames.SQLite;
    }
}
