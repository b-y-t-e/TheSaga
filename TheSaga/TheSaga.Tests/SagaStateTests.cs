using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Threading.Tasks;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Persistance;
using TheSaga.Registrator;
using TheSaga.SagaStates;
using TheSaga.Tests.Sagas.OrderTestSaga;
using TheSaga.Tests.Sagas.OrderTestSaga.Activities;
using TheSaga.Tests.Sagas.OrderTestSaga.EventHandlers;
using TheSaga.Tests.Sagas.OrderTestSaga.Events;
using TheSaga.Tests.Sagas.OrderTestSaga.States;
using Xunit;

namespace TheSaga.Tests
{
    public class SagaStateTests
    {
        [Fact]
        public async Task WHEN_invalidEvent_THEN_sagaShouldIgnoreThatEvent()
        {
            // given
            ISagaState newSagaState = await sagaCoordinator.
                Send(new OrderCreatedEvent());

            IEvent invalidEvent = new OrderSendEvent()
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
            persistedState.CurrentState.ShouldBe(nameof(StateCreated));
            persistedState.CurrentStep.ShouldBe(null);
            persistedState.Logs.ShouldContain(nameof(OrderCreatedEvent));
            persistedState.Logs.Count.ShouldBe(2);
        }

        [Fact]
        public async Task WHEN_someNextEvent_THEN_sagaShouldBeInValidState()
        {
            // given
            ISagaState newSagaState = await sagaCoordinator.
                Send(new OrderCreatedEvent());

            IEvent skompletowanoEvent = new OrderCompletedEvent()
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
            persistedState.CurrentState.ShouldBe(nameof(StateCompleted));
            persistedState.CurrentStep.ShouldBe(null);
            persistedState.Logs.ShouldContain(nameof(OrderCreatedEvent));
            persistedState.Logs.ShouldContain(nameof(OrderCreatedEventHandler));
            persistedState.Logs.ShouldContain(nameof(OrderCompletedEvent));
            persistedState.Logs.ShouldContain(nameof(SendEmailToClientEvent));
            persistedState.Logs.ShouldContain(nameof(SendMessageToTheManagerEvent));
            persistedState.Logs.ShouldContain(nameof(OrderCourierEvent));
            persistedState.Logs.Count.ShouldBe(6);
        }

        [Fact]
        public async Task WHEN_startEvent_THEN_sagaShouldBeCreated()
        {
            // given
            IEvent startEvent = new OrderCreatedEvent();

            // when
            ISagaState sagaState = await sagaCoordinator.
                Send(startEvent);

            // then
            OrderState persistedState = (OrderState)await sagaPersistance.
                Get(sagaState.CorrelationID);

            persistedState.ShouldNotBeNull();
            persistedState.CurrentStep.ShouldBe(null);
            persistedState.CurrentState.ShouldBe(nameof(StateCreated));
            persistedState.CorrelationID.ShouldBe(sagaState.CorrelationID);
            persistedState.Logs.ShouldContain(nameof(OrderCreatedEvent));
        }

        #region Arrange

        private ISagaCoordinator sagaCoordinator;
        private ISagaPersistance sagaPersistance;
        private ISagaRegistrator sagaRegistrator;
        private IServiceProvider serviceProvider;

        public SagaStateTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddScoped<ISagaPersistance, InMemorySagaPersistance>();
            services.AddScoped<ISagaRegistrator, SagaRegistrator>();
            services.AddScoped<ISagaCoordinator, SagaCoordinator>();
            serviceProvider = services.BuildServiceProvider();

            sagaRegistrator = serviceProvider.
                GetRequiredService<ISagaRegistrator>();

            sagaPersistance = serviceProvider.
                GetRequiredService<ISagaPersistance>();

            sagaCoordinator = serviceProvider.
                GetRequiredService<ISagaCoordinator>();

            sagaRegistrator.Register(
                new OrderSagaDefinition().GetModel(serviceProvider));
        }

        #endregion Arrange
    }
}