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
using TheSaga.States;
using TheSaga.Tests.Sagas.SyncAndInvalidSaga;
using TheSaga.Tests.Sagas.SyncAndInvalidSaga.EventHandlers;
using TheSaga.Tests.Sagas.SyncAndInvalidSaga.Events;
using TheSaga.Tests.Sagas.SyncAndInvalidSaga.Exceptions;
using TheSaga.Tests.Sagas.SyncAndInvalidSaga.States;
using Xunit;

namespace TheSaga.Tests
{
    public class SyncAndInvalidSagaTests
    {
        [Fact]
        public async Task WHEN_sagaThrowsErrorOnStart_THEN_sagaStepExceptionIsThrown()
        {
            // given
            Guid correlationID = Guid.NewGuid();
            IEvent startEvent = new InvalidCreatedEvent()
            {
                CorrelationID = correlationID
            };

            // then
            await Assert.ThrowsAsync<SagaStepException>(async () =>
            {
                // when
                ISagaState sagaState = await sagaCoordinator.
                    Send(startEvent);
            });
        }

        [Fact]
        public async Task WHEN_sagaThrowsErrorOnStart_THEN_sagaShouldNotExists()
        {
            // given
            Guid correlationID = Guid.NewGuid();
            IEvent startEvent = new InvalidCreatedEvent()
            {
                CorrelationID = correlationID
            };

            // when
            await Assert.ThrowsAsync<SagaStepException>(async () =>
            {
                ISagaState sagaState = await sagaCoordinator.
                    Send(startEvent);
            });

            // then
            ISagaState persistedState = await sagaPersistance.Get(correlationID);
            persistedState.ShouldBeNull();
        }

        [Fact]
        public async Task WHEN_sagaThrowsErrorOnUpdate_THEN_sagaShouldBeInValidState()
        {
            // given
            ISagaState sagaState = await sagaCoordinator.
                Send(new ValidCreatedEvent());

            // when
            await Assert.ThrowsAsync<SagaStepException>(async () =>
            {
                await sagaCoordinator.Send(new InvalidUpdateEvent()
                {
                    CorrelationID = sagaState.CorrelationID
                });
            });

            // then
            InvalidSagaState persistedState = (InvalidSagaState)await sagaPersistance.
                Get(sagaState.CorrelationID);

            persistedState.ShouldNotBeNull();
            persistedState.CurrentStep.ShouldBe(null);
            persistedState.CurrentState.ShouldBe(nameof(StateCreated));
            persistedState.CurrentError.ShouldNotBeNull();
            persistedState.CorrelationID.ShouldBe(sagaState.CorrelationID);
            persistedState.Logs.ShouldContain("execution3");
            persistedState.Logs.ShouldContain("compensation3");
            persistedState.Logs.ShouldContain("execution2");
            persistedState.Logs.ShouldContain("compensation2");
            persistedState.Logs.ShouldContain("execution1");
            persistedState.Logs.ShouldContain("compensation1");
        }

        #region Arrange

        private ISagaCoordinator sagaCoordinator;
        private ISagaPersistance sagaPersistance;
        private ISagaRegistrator sagaRegistrator;
        private IServiceProvider serviceProvider;

        public SyncAndInvalidSagaTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddTheSaga();

            serviceProvider = services.BuildServiceProvider();

            sagaRegistrator = serviceProvider.
                GetRequiredService<ISagaRegistrator>();

            sagaPersistance = serviceProvider.
                GetRequiredService<ISagaPersistance>();

            sagaCoordinator = serviceProvider.
                GetRequiredService<ISagaCoordinator>();

            sagaRegistrator.Register(
                new InvalidSagaDefinition().GetModel(serviceProvider));
        }

        #endregion Arrange
    }
}