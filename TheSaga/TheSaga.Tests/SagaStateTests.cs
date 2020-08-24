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
using TheSaga.Tests.Sagas.OrderTestSaga.Events;
using TheSaga.Tests.Sagas.OrderTestSaga.States;
using TheSaga.Exceptions;

namespace TheSaga.Tests
{
    public class SagaStateTests
    {
        IServiceProvider serviceProvider;

        [Fact]
        public async Task WHEN_startEvent_THEN_sagaShouldBeCreated()
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
            persistedState.CurrentStep.ShouldBe(null);
            persistedState.CurrentState.ShouldBe(nameof(Nowe));
            persistedState.CorrelationID.ShouldBe(sagaState.CorrelationID);
        }


        [Fact]
        public async Task WHEN_invalidEvent_THEN_sagaShouldIgnoreThatEvent()
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

            var newSataState = await sagaCoordinator.
                Process(new Utworzone());

            IEvent invalidEvent = new Wyslano()
            {
                CorrelationID = newSataState.CorrelationID
            };

            // then
            await Assert.ThrowsAsync<SagaInvalidEventForStateException>(() =>
                // when
                sagaCoordinator.
                    Process(invalidEvent)
            );

            // then
            var persistedState = await sagaPersistance.Get(newSataState.CorrelationID);
            persistedState.ShouldNotBeNull();
            persistedState.CurrentState.ShouldBe(nameof(Nowe));
            persistedState.CurrentStep.ShouldBe(null);
        }


        public SagaStateTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddScoped<ISagaPersistance, InMemorySagaPersistance>();
            services.AddScoped<ISagaRegistrator, SagaRegistrator>();
            services.AddScoped<ISagaCoordinator, SagaCoordinator>();
            serviceProvider = services.BuildServiceProvider();
        }
    }

}
