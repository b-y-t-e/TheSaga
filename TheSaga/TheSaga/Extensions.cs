using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using TheSaga.Builders;
using TheSaga.Commands.Handlers;
using TheSaga.Config;
using TheSaga.Coordinators;
using TheSaga.Locking;
using TheSaga.Locking.InMemory;
using TheSaga.Messages.MessageBus;
using TheSaga.Observables.Registrator;
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
            services.AddSingleton<ObservableRegistrator>();
            services.AddSingleton<IMessageBus, InMemoryMessageBus>();
            services.AddSingleton<ISagaPersistance, InMemorySagaPersistance>();
            services.AddSingleton<ISagaLocking, InMemorySagaLocking>();
            services.AddSingleton<IDateTimeProvider, LocalDateTimeProvider>();

            services.AddTransient<ISagaRegistrator, SagaRegistrator>();
            services.AddTransient<ISagaCoordinator, SagaCoordinator>();

            services.AddTransient(typeof(ISagaBuilder<>), typeof(SagaBuilder<>));
            services.AddTransient<ExecuteSagaCommandHandler>();
            services.AddTransient<ExecuteActionCommandHandler>();
            services.AddTransient<ExecuteStepCommandHandler>();

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

        public static IServiceProvider ResumeSagas(
            this IServiceProvider provider)
        {
            ISagaCoordinator coordinator = provider.
                GetRequiredService<ISagaCoordinator>();

            coordinator.ResumeAll();

            return provider;
        }
    }
}