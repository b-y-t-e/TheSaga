using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheSaga.Builders;
using TheSaga.Commands.Handlers;
using TheSaga.Config;
using TheSaga.Coordinators;
using TheSaga.Errors;
using TheSaga.Locking;
using TheSaga.Locking.InMemory;
using TheSaga.MessageBus;
using TheSaga.MessageBus.InMemory;
using TheSaga.MessageBus.Interfaces;
using TheSaga.ModelsSaga.Interfaces;
using TheSaga.Observables.Registrator;
using TheSaga.Persistance;
using TheSaga.Persistance.InMemory;
using TheSaga.Providers;
using TheSaga.Providers.Interfaces;
using TheSaga.Registrator;
using TheSaga.Registrator.Interfaces;

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
            services.AddSingleton<IAsyncSagaErrorHandler, AsyncSagaErrorHandler>();

            services.AddTransient<ISagaRegistrator, SagaRegistrator>();
            services.AddTransient<ISagaCoordinator, SagaCoordinator>();

            services.AddTransient(typeof(ISagaBuilder<>), typeof(SagaBuilder<>));
            //services.AddTransient<ExecuteSagaCommandHandler>();
            services.AddTransient<ExecuteActionCommandHandler>();
            services.AddTransient<ExecuteStepCommandHandler>();

            services.AddSagaModelDefinitions();

            if (configAction != null)
                configAction(new TheSagaConfig
                {
                    Services = services
                });

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

        public static async Task<IServiceProvider> ResumeSagas(
            this IServiceProvider provider)
        {
            ISagaCoordinator coordinator = provider.
                GetRequiredService<ISagaCoordinator>();

            await coordinator.ResumeAll();

            return provider;
        }
    }
}
