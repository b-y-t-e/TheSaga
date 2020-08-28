using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using TheSaga.Builders;
using TheSaga.Config;
using TheSaga.Coordinators;
using TheSaga.Execution;
using TheSaga.Execution.Commands;
using TheSaga.Execution.Commands.Handlers;
using TheSaga.Locking;
using TheSaga.Locking.InMemory;
using TheSaga.Messages.MessageBus;
using TheSaga.Persistance;
using TheSaga.Persistance.InMemory;
using TheSaga.Providers;
using TheSaga.Registrator;
using TheSaga.SagaModels;
using TheSaga.Utils;

[assembly: InternalsVisibleTo("TheSaga.Tests")]

namespace TheSaga
{
    public static class Extensions
    {
        public static IServiceCollection AddSaga(
            this IServiceCollection services,
            Action<ITheSagaConfig> configAction = null)
        {
            services.AddSingleton<IInternalMessageBus, InternalMessageBus>();
            services.AddSingleton<ISagaPersistance, InMemorySagaPersistance>();
            services.AddSingleton<ISagaRegistrator, SagaRegistrator>();
            services.AddSingleton<ISagaCoordinator, SagaCoordinator>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddSingleton<ISagaLocking, InMemorySagaLocking>();

            services.AddSingleton(typeof(ISagaBuilder<>), typeof(SagaBuilder<>));
            services.AddTransient(typeof(ExecuteSagaCommandHandler), typeof(ExecuteSagaCommandHandler));
            services.AddTransient(typeof(ExecuteActionCommandHandler), typeof(ExecuteActionCommandHandler));
            services.AddTransient(typeof(ExecuteStepCommandHandler), typeof(ExecuteStepCommandHandler));

            services.AddSagaModelDefinitions();

            if (configAction != null)
            {
                configAction(new TheSagaConfig()
                {
                    Services = services
                });
            }

            return services;
        }

        public static ITheSagaConfig AddModelDefinitions(
            this ITheSagaConfig config)
        {
            config.Services.AddSagaModelDefinitions();
            return config;
        }

        public static IServiceCollection AddSagaModelDefinitions(
            this IServiceCollection services)
        {
            services.Scan(s =>
                s.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                    .AddClasses(c => c.AssignableTo(typeof(ISagaModelBuilder<>)))
                    .AsSelfWithInterfaces()
                    .WithTransientLifetime());

            return services;
        }

       /* public static IServiceProvider RegisterSagaModelDefinitions(
            this IServiceProvider provider)
        {
            var modelDefinitions = AppDomain.CurrentDomain.GetAssemblies().
                SelectMany(a => a.GetTypes()).
                Where(t => t.Is(typeof(ISagaModelDefintion<>))).
                ToArray();

            var registrator = provider.GetRequiredService<ISagaRegistrator>();
            foreach (var modelDefinition in modelDefinitions)
            {
                registrator.Register(
             }

            return provider;
        }*/
    }
}