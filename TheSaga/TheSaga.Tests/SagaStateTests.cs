using System;
using System.Threading;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Builders;
using TheSaga.Coordinators;
using TheSaga.Executors;
using TheSaga.Interfaces;
using TheSaga.Models;
using TheSaga.Registrator;
using TheSaga.Persistance;
using TheSaga.States;
using Xunit;
using Xunit.Sdk;
using TheSaga.Tests.Sagas.OrderTestSaga;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace TheSaga.Tests
{
    public class SagaStateTests
    {
        IServiceProvider serviceProvider;


        [Fact]
        public async Task WHEN_startEventIsProcessed_THEN_sagaShouldBeCreated()
        {
            // given
            ISagaCoordinator sagaCoordinator = serviceProvider.
                GetRequiredService<ISagaCoordinator>();

            ISagaRegistrator sagaRegistrator = serviceProvider.
                GetRequiredService<ISagaRegistrator>();

            ISagaPersistance sagaPersistance = serviceProvider.
                GetRequiredService<ISagaPersistance>();

            sagaRegistrator.Register(
                new OrderSagaDefinition().GetModel());

            IEvent startEvent = new Utworzone();

            // when
            ISagaState sagaState = await sagaCoordinator.
                Process(startEvent);

            // then
            var persistedState = await sagaPersistance.Get(sagaState.CorrelationID);
            persistedState.ShouldNotBeNull();
            persistedState.CurrentState.ShouldBe(nameof(Nowe));
            persistedState.CurrentStep.ShouldBe(null);
        }


        public SagaStateTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddScoped<ISagaPersistance, InMemorySagaPersistance>();
            services.AddScoped<ISagaRegistrator, SagaRegistrator>();
            services.AddTransient<ISagaCoordinator, SagaCoordinator>();
            serviceProvider = services.BuildServiceProvider();
        }
    }

}
