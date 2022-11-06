using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
using TheSaga.Options;
using TheSaga.Persistance;
using TheSaga.Persistance.InFile;
using TheSaga.Persistance.InMemory;
using TheSaga.Providers;
using TheSaga.Providers.Interfaces;
using TheSaga.Registrator;
using TheSaga.Registrator.Interfaces;
using TheSaga.Models.Interfaces;
using TheSaga.Serializer;

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
            services.AddTransient<ISagaSerializer, SagaSerializer>();
            services.AddSingleton<IMessageBus, InMemoryMessageBus>();
            services.AddSingleton<ISagaPersistance, InMemorySagaPersistance>();
            services.AddSingleton<ISagaLocking, InMemorySagaLocking>();
            services.AddSingleton<IDateTimeProvider, LocalDateTimeProvider>();
            services.AddSingleton<IAsyncSagaErrorHandler, AsyncSagaErrorHandler>();
            services.AddSingleton<ILogger>(r =>
            {
                var loggerFactory = r.GetService<ILoggerFactory>();
                if (loggerFactory != null)
                    return loggerFactory.CreateLogger("TheSaga");
                return new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider().CreateLogger("TheSaga");
            });
            services.AddTransient<ISagaRegistrator, SagaRegistrator>();
            services.AddTransient<ISagaCoordinator, SagaCoordinator>();

            services.AddTransient(typeof(ISagaBuilder<>), typeof(SagaBuilder<>));
            //services.AddTransient<ExecuteSagaCommandHandler>();
            services.AddTransient<ExecuteActionCommandHandler>();
            services.AddTransient<ExecuteStepCommandHandler>();
            services.AddTransient<CalculateNextStepHandler>();
            
            services.AddSagaModelDefinitions();

            if (configAction != null)
                configAction(new TheSagaConfig
                {
                    Services = services
                });

            return services;
        }

        public static void AddBeforeExecuteMiddlewares<T>(
            this ITheSagaConfig config)
            where T : ISagaMiddleware
        {
            Middlewares.AddBeforeExecuteMiddlewares<T>();
        }

        public static void AddAfterExecuteMiddlewares<T>(
            this ITheSagaConfig config)
            where T : ISagaMiddleware
        {
            Middlewares.AddAfterExecuteMiddlewares<T>();
        }

        public static void AddBeforeRequestCallback<T>(
            this ITheSagaConfig config)
            where T : ISagaBeforeRequestCallback
        {
            Callbacks.AddBeforeRequestCallback<T>();
        }

        public static void AddAfterRequestCallback<T>(
            this ITheSagaConfig config)
            where T : ISagaAfterRequestCallback
        {
            Callbacks.AddAfterRequestCallback<T>();
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
        public static void UseFilePersistance(this ITheSagaConfig config)
        {
            config.Services.AddTransient<ISagaPersistance, InFileSagaPersistance>();
        }
    }
}
