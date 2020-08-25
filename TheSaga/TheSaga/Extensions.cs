using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TheSaga.Coordinators;
using TheSaga.Messages.MessageBus;
using TheSaga.Persistance;
using TheSaga.Registrator;

namespace TheSaga
{
    public static class Extensions
    {
        public static IServiceCollection AddTheSaga(this IServiceCollection services)
        {
            services.AddScoped<ISagaPersistance, InMemorySagaPersistance>();
            services.AddScoped<ISagaRegistrator, SagaRegistrator>();
            services.AddScoped<ISagaCoordinator, SagaCoordinator>();
            services.AddScoped<IInternalMessageBus, InternalMessageBus>();

            return services;
        }
    }
}
