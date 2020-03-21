using Autofac;
using Autofac.Core;
using FluentValidation;
using System;
using System.Linq;
using System.Reflection;
using TauCode.Cqrs.Autofac;
using TauCode.Cqrs.Commands;
using TauCode.Cqrs.Queries;
using TauCode.Cqrs.Validation;

namespace TauCode.WebApi.Server.Cqrs
{
    public static class WebApiServerCqrsExtensions
    {
        public static ContainerBuilder AddCqrs(
            this ContainerBuilder containerBuilder,
            Assembly cqrsAssembly,
            Type commandHandlerDecoratorType)
        {
            if (containerBuilder == null)
            {
                throw new ArgumentNullException(nameof(containerBuilder));
            }

            if (cqrsAssembly == null)
            {
                throw new ArgumentNullException(nameof(cqrsAssembly));
            }

            if (commandHandlerDecoratorType == null)
            {
                throw new ArgumentNullException(nameof(commandHandlerDecoratorType));
            }

            // command dispatching
            containerBuilder
                .RegisterType<CommandDispatcher>()
                .As<ICommandDispatcher>()
                .InstancePerLifetimeScope();

            containerBuilder
                .RegisterType<ValidatingCommandDispatcher>()
                .As<IValidatingCommandDispatcher>()
                .InstancePerLifetimeScope();

            containerBuilder
                .RegisterType<AutofacCommandHandlerFactory>()
                .As<ICommandHandlerFactory>()
                .InstancePerLifetimeScope();

            // register API ICommandHandler decorator
            containerBuilder
                .RegisterAssemblyTypes(cqrsAssembly)
                .Where(t => t.IsClosedTypeOf(typeof(ICommandHandler<>)))
                .As(t => t.GetInterfaces()
                    .Where(x => x.IsClosedTypeOf(typeof(ICommandHandler<>)))
                    .Select(x => new KeyedService("commandHandler", x)))
                .InstancePerLifetimeScope();

            containerBuilder
                .RegisterGenericDecorator(
                    commandHandlerDecoratorType,
                    typeof(ICommandHandler<>),
                    "commandHandler");

            // command validator source
            containerBuilder
                .RegisterInstance(new CommandValidatorSource(cqrsAssembly))
                .As<ICommandValidatorSource>()
                .SingleInstance();

            // validators
            containerBuilder
                .RegisterAssemblyTypes(cqrsAssembly)
                .Where(t => t.IsClosedTypeOf(typeof(AbstractValidator<>)))
                .AsSelf()
                .InstancePerLifetimeScope();

            // query handling
            containerBuilder
                .RegisterType<QueryRunner>()
                .As<IQueryRunner>()
                .InstancePerLifetimeScope();

            containerBuilder
                .RegisterType<ValidatingQueryRunner>()
                .As<IValidatingQueryRunner>()
                .InstancePerLifetimeScope();

            containerBuilder
                .RegisterType<AutofacQueryHandlerFactory>()
                .As<IQueryHandlerFactory>()
                .InstancePerLifetimeScope();

            containerBuilder
                .RegisterAssemblyTypes(cqrsAssembly)
                .Where(t => t.IsClosedTypeOf(typeof(IQueryHandler<>)))
                .AsImplementedInterfaces()
                .AsSelf()
                .InstancePerLifetimeScope();

            // query validator source
            containerBuilder
                .RegisterInstance(new QueryValidatorSource(cqrsAssembly))
                .As<IQueryValidatorSource>()
                .SingleInstance();

            return containerBuilder;
        }
    }
}
