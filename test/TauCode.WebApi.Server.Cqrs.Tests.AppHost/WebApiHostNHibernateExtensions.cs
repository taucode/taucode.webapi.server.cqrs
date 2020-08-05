using Autofac;
using FluentNHibernate.Cfg;
using FluentNHibernate.Conventions.Helpers;
using Inflector;
using NHibernate;
using NHibernate.Cfg;
using System;
using System.Reflection;
using TauCode.Domain.NHibernate.Conventions;

// todo: rename. not "Host" but "Server"
namespace TauCode.WebApi.Server.Cqrs.Tests.AppHost
{
    public static class WebApiHostNHibernateExtensions
    {
        public static ISessionFactory BuildSessionFactory(
            Configuration configuration,
            Assembly mappingsAssembly,
            Type idUserTypeGenericType)
        {
            return Fluently.Configure(configuration)
                .Mappings(m => m.FluentMappings.AddFromAssembly(mappingsAssembly)
                    .Conventions.Add(ForeignKey.Format((p, t) =>
                    {
                        if (p == null) return t.Name.Underscore() + "_id";

                        return p.Name.Underscore() + "_id";
                    }))
                    .Conventions.Add(LazyLoad.Never())
                    .Conventions.Add(Table.Is(x => x.TableName.Underscore().ToUpper()))
                    .Conventions.Add(ConventionBuilder.Property.Always(x => x.Column(x.Property.Name.Underscore())))
                    .Conventions.Add(new IdUserTypeConvention(idUserTypeGenericType))
                )
                .BuildSessionFactory();
        }

        public static ContainerBuilder AddNHibernate(
            this ContainerBuilder containerBuilder,
            Configuration configuration,
            Assembly mappingsAssembly,
            Type idUserTypeGenericType)
        {
            containerBuilder
                .Register(c => BuildSessionFactory(configuration, mappingsAssembly, idUserTypeGenericType))
                .As<ISessionFactory>()
                .SingleInstance();

            containerBuilder
                .Register(c => c.Resolve<ISessionFactory>().OpenSession())
                .As<ISession>()
                .InstancePerLifetimeScope();

            return containerBuilder;
        }
    }
}
