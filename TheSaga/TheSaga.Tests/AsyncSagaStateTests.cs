using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Threading.Tasks;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.Persistance;
using TheSaga.Registrator;
using TheSaga.SagaStates;
using TheSaga.States;
using TheSaga.Tests.Sagas.AsyncSaga;
using TheSaga.Tests.Sagas.AsyncSaga.EventHandlers;
using TheSaga.Tests.Sagas.AsyncSaga.Events;
using Xunit;

namespace TheSaga.Tests
{
    public class AsyncSagaStateTests
    {
        [Fact]
        public async Task WHEN_abc_THEN_def()
        {
            // given
            IEvent startEvent = new CreatedEvent();

            // when
            ISagaState sagaState = await sagaCoordinator.
                Send(startEvent);

            // then
            AsyncState persistedState = (AsyncState)await sagaPersistance.
                Get(sagaState.CorrelationID);

            persistedState.ShouldNotBeNull();
            persistedState.CurrentStep.ShouldStartWith("~");
            persistedState.CurrentState.ShouldBe("~");
            persistedState.CorrelationID.ShouldBe(sagaState.CorrelationID);
            persistedState.Logs.ShouldContain(nameof(CreatedEventHandler));
            persistedState.Logs.Count.ShouldBe(1);
        }

        [Fact]
        public async Task WHEN_sagaContainsAsyncMethods_THEN_stepsShouldBeExecutedAsync()
        {
            // given
            IEvent startEvent = new CreatedEvent();

            ISagaState sagaState = await sagaCoordinator.
                Send(startEvent);

            // when
            await sagaCoordinator.
                WaitForState<SagaFinishState>(sagaState.CorrelationID);

            // then
            AsyncState persistedState = (AsyncState)await sagaPersistance.
                Get(sagaState.CorrelationID);

            persistedState.ShouldNotBeNull();
            persistedState.CurrentStep.ShouldStartWith("~");
            persistedState.CurrentState.ShouldBe("~");
            persistedState.CorrelationID.ShouldBe(sagaState.CorrelationID);
            persistedState.Logs.ShouldContain(nameof(CreatedEventHandler));
            persistedState.Logs.Count.ShouldBe(1);
        }

        #region Arrange

        private ISagaCoordinator sagaCoordinator;
        private ISagaPersistance sagaPersistance;
        private ISagaRegistrator sagaRegistrator;
        private IServiceProvider serviceProvider;

        public AsyncSagaStateTests()
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
                new AsyncSagaDefinition().GetModel(serviceProvider));
        }

        #endregion Arrange
    }
}