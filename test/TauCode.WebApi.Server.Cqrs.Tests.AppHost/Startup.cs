using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

// todo
namespace TauCode.WebApi.Server.Cqrs.Tests.AppHost
{
    public class Startup : IAutofacStartup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public ILifetimeScope AutofacContainer { get; private set; }
        public SQLiteTestConfigurationBuilder SQLiteTestConfigurationBuilder { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var cqrsAssembly = typeof(Startup).Assembly;
            services
                .AddControllers(options => options.Filters.Add(new ValidationFilterAttribute(cqrsAssembly)))
                .AddNewtonsoftJson(options => options.UseCamelCasing(false));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "Demo Server RESTful Service",
                        Version = "v1"
                    });
                c.CustomSchemaIds(x => x.FullName);
                c.EnableAnnotations();
            });
            services.AddSwaggerGenNewtonsoftSupport();
        }

        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            throw new NotImplementedException();

            //var cqrsAssembly = typeof(Startup).Assembly;

            //this.SQLiteTestConfigurationBuilder = new SQLiteTestConfigurationBuilder();
            //var migrator = new FluentDbMigrator(
            //    DbProviderNames.SQLite,
            //    this.SQLiteTestConfigurationBuilder.ConnectionString, 
            //    typeof(M0_Baseline).Assembly);
            //migrator.Migrate();

            //using (var connection = new SQLiteConnection(this.SQLiteTestConfigurationBuilder.ConnectionString))
            //{
            //    connection.Open();
            //    connection.BoostSQLiteInsertions();

            //    // todo: without following table drop, deserialization won't work due to VersionInfo table. consider adding filtering predicate to 'DeserializeDbData'. comment GUID: 9ed2372e-f5f0-4a03-8a9f-5deb2ff464a4 (see dbdata.json)
            //    using (var command = connection.CreateCommand())
            //    {
            //        command.CommandText = "DROP TABLE VersionInfo";
            //        command.ExecuteNonQuery();
            //    }

            //    var json = typeof(Startup).Assembly.GetResourceText(".dbdata.json", true);

            //    var dbSerializer = SQLiteUtilityFactory.Instance.CreateSerializer(connection, null);
            //    dbSerializer.DeserializeDbData(json);
            //}

            //containerBuilder
            //    .AddCqrs(cqrsAssembly, typeof(TransactionalCommandHandlerDecorator<>))
            //    .AddNHibernate(
            //        this.SQLiteTestConfigurationBuilder.Configuration,
            //        typeof(Startup).Assembly,
            //        typeof(SQLiteIdUserType<>));

            //containerBuilder
            //    .RegisterAssemblyTypes(typeof(Startup).Assembly)
            //    .Where(t => t.FullName.EndsWith("Repository"))
            //    .AsImplementedInterfaces()
            //    .InstancePerLifetimeScope();

            //containerBuilder
            //    .RegisterInstance(this)
            //    .As<IAutofacStartup>()
            //    .SingleInstance();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo Server RESTful Service"); });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
