using Convey;
using Microsoft.Extensions.DependencyInjection;
using System;
using TheSaga.Config;
using TheSaga.ModelsSaga.Steps;

namespace TheSaga.Integrations.Convey
{
    public static class Extensions
    {
        public static IConveyBuilder AddSagaConveyIntegration(this IConveyBuilder builder)
        {
            builder.Services.AddTransient(
                typeof(ISagaPublishActivity<,,>),
                typeof(ConveySagaStepForPublishActivity<,,>));

            return builder;
        }
    }
}
