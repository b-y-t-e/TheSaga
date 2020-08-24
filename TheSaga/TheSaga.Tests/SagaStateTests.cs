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
using TheSaga.Tests.Sagas.OrderTestSaga.Activities;

namespace TheSaga.Tests
{
    public class SagaStateTests
    {
        IServiceProvider serviceProvider;

        [Fact]
        public async Task WHEN_startEvent_THEN_sagaShouldBeCreated()
        {
            // given
            ISagaPersistance sagaPersistance = serviceProvider.
                GetRequiredService<ISagaPersistance>();

            ISagaCoordinator sagaCoordinator = serviceProvider.
                GetRequiredService<ISagaCoordinator>();

            IEvent startEvent = new Utworzone();

            // when
            ISagaState sagaState = await sagaCoordinator.
                Send(startEvent);

            // then
            OrderState persistedState = (OrderState)await sagaPersistance.
                Get(sagaState.CorrelationID);

            persistedState.ShouldNotBeNull();
            persistedState.CurrentStep.ShouldBe(null);
            persistedState.CurrentState.ShouldBe(nameof(Nowe));
            persistedState.CorrelationID.ShouldBe(sagaState.CorrelationID);
            persistedState.Logi.ShouldContain(nameof(Utworzone));
        }


        [Fact]
        public async Task WHEN_invalidEvent_THEN_sagaShouldIgnoreThatEvent()
        {
            // given
            ISagaPersistance sagaPersistance = serviceProvider.
                GetRequiredService<ISagaPersistance>();

            ISagaCoordinator sagaCoordinator = serviceProvider.
                GetRequiredService<ISagaCoordinator>();

            ISagaState newSagaState = await sagaCoordinator.
                Send(new Utworzone());

            IEvent invalidEvent = new Wyslano()
            {
                CorrelationID = newSagaState.CorrelationID
            };

            // then
            await Assert.ThrowsAsync<SagaInvalidEventForStateException>(() =>
                // when
                sagaCoordinator.
                    Send(invalidEvent)
            );

            // then
            OrderState persistedState = (OrderState)await sagaPersistance.
                Get(newSagaState.CorrelationID);

            persistedState.ShouldNotBeNull();
            persistedState.CurrentState.ShouldBe(nameof(Nowe));
            persistedState.CurrentStep.ShouldBe(null);
            persistedState.Logi.ShouldContain(nameof(Utworzone));
            persistedState.Logi.Count.ShouldBe(1);
        }

        [Fact]
        public async Task WHEN_someNextEvent_THEN_sagaShouldBeInValidState()
        {
            // given
            ISagaPersistance sagaPersistance = serviceProvider.
                GetRequiredService<ISagaPersistance>();

            ISagaCoordinator sagaCoordinator = serviceProvider.
                GetRequiredService<ISagaCoordinator>();

            var newSagaState = await sagaCoordinator.
                Send(new Utworzone());

            IEvent skompletowanoEvent = new Skompletowano()
            {
                CorrelationID = newSagaState.CorrelationID
            };

            // then
            await sagaCoordinator.
                Send(skompletowanoEvent);

            // then
            OrderState persistedState = (OrderState)await sagaPersistance.
                Get(newSagaState.CorrelationID);

            persistedState.ShouldNotBeNull();
            persistedState.CurrentState.ShouldBe(nameof(Skompletowane));
            persistedState.CurrentStep.ShouldBe(null);
            persistedState.Logi.ShouldContain(nameof(Utworzone));
            persistedState.Logi.ShouldContain(nameof(Skompletowano));
            persistedState.Logi.ShouldContain(nameof(WyslijEmailDoKlienta));
            persistedState.Logi.ShouldContain(nameof(WyslijWiadomoscDoKierownika));
            persistedState.Logi.ShouldContain(nameof(ZamowKuriera));
            persistedState.Logi.Count.ShouldBe(5);
        }

        public SagaStateTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddScoped<ISagaPersistance, InMemorySagaPersistance>();
            services.AddScoped<ISagaRegistrator, SagaRegistrator>();
            services.AddScoped<ISagaCoordinator, SagaCoordinator>();
            serviceProvider = services.BuildServiceProvider();

            ISagaRegistrator sagaRegistrator = serviceProvider.
                GetRequiredService<ISagaRegistrator>();

            sagaRegistrator.Register(
                new OrderSagaDefinition().GetModel());

        }
    }

}
