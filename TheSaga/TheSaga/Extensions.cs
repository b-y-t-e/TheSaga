using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.CompilerServices;
using TheSaga.Config;
using TheSaga.Coordinators;
using TheSaga.Execution;
using TheSaga.Execution.Actions;
using TheSaga.Execution.Steps;
using TheSaga.InternalMessages.MessageBus;
using TheSaga.Persistance;
using TheSaga.Persistance.InMemory;
using TheSaga.Providers;
using TheSaga.Registrator;
using TheSaga.Utils;

[assembly: InternalsVisibleTo("TheSaga.Tests")]

namespace TheSaga
{
    public static class Extensions
    {
        public static IServiceCollection AddTheSaga(
            this IServiceCollection services,
            Action<ITheSagaConfig> configAction = null)
        {
            services.AddSingleton<IInternalMessageBus, InternalMessageBus>();
            services.AddSingleton<ISagaPersistance, InMemorySagaPersistance>();
            services.AddSingleton<ISagaRegistrator, SagaRegistrator>();
            services.AddSingleton<ISagaCoordinator, SagaCoordinator>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddSingleton<ISagaLocking, InMemorySagaLocking>();
            
            services.AddTransient(typeof(SagaExecutor<>), typeof(SagaExecutor<>));
            services.AddTransient(typeof(SagaActionExecutor<>), typeof(SagaActionExecutor<>));
            services.AddTransient(typeof(SagaStepExecutor<>), typeof(SagaStepExecutor<>));

            if (configAction != null)
            {
                configAction(new TheSagaConfig()
                {
                    Services = services
                });
            }

            return services;
        }
    }
}