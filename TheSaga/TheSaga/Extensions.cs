using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TheSaga.Coordinators;
using TheSaga.InternalMessages.MessageBus;
using TheSaga.Persistance;
using TheSaga.Registrator;

[assembly: InternalsVisibleTo("TheSaga.Tests")]
namespace TheSaga
{
    public static class Extensions
    {
        public static IServiceCollection AddTheSaga(this IServiceCollection services)
        {
            services.AddSingleton<IInternalMessageBus, InternalMessageBus>();
            services.AddSingleton<ISagaPersistance, InMemorySagaPersistance>();
            services.AddSingleton<ISagaRegistrator, SagaRegistrator>();
            services.AddSingleton<ISagaCoordinator, SagaCoordinator>();
            return services;
        }
    }
}
