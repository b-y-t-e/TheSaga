using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Threading.Tasks;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.Persistance;
using TheSaga.Persistance.SqlServer;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.Registrator;
using TheSaga.SagaStates;
using TheSaga.Tests.Sagas.SyncAndInvalidSaga;
using TheSaga.Tests.Sagas.SyncAndInvalidSaga.Events;
using TheSaga.Tests.Sagas.SyncAndInvalidSaga.Exceptions;
using TheSaga.Tests.Sagas.SyncAndInvalidSaga.States;
using Xunit;

namespace TheSaga.Tests
{
    public class SyncAndInvalidSagaTests
    {
        [Fact]
        public async Task WHEN_compensationThrowsErrorOnUpdate_THEN_sagaShouldBeInValidState()
        {
            // given
            ISagaData sagaData = await sagaCoordinator.
                Send(new ValidCreatedEvent());

            // when
            await Assert.ThrowsAsync<TestCompensationException>(async () =>
            {
                await sagaCoordinator.Send(new InvalidCompensationEvent()
                {
                    CorrelationID = sagaData.CorrelationID
                });
            });

            // then
            InvalidSagaData persistedState = (InvalidSagaData)await sagaPersistance.
                Get(sagaData.CorrelationID);

            persistedState.ShouldNotBeNull();
            persistedState.SagaState.CurrentStep.ShouldBe(null);
            persistedState.SagaState.CurrentState.ShouldBe(nameof(StateCreated));
            persistedState.SagaState.CurrentError.ShouldNotBeNull();
            persistedState.CorrelationID.ShouldBe(sagaData.CorrelationID);

            persistedState.SagaInfo.History.ShouldContain(item =>
                item.IsCompensating == true && item.StepName == "InvalidCompensationEventStep1");

            persistedState.SagaInfo.History.ShouldContain(item =>
                item.IsCompensating == false && item.StepName == "InvalidCompensationEventStep1");

            persistedState.SagaInfo.History.ShouldContain(item =>
                item.IsCompensating == true && item.StepName == "InvalidCompensationEventStep2");

            persistedState.SagaInfo.History.ShouldContain(item =>
                item.IsCompensating == false && item.StepName == "InvalidCompensationEventStep2");

            persistedState.SagaInfo.History.ShouldContain(item =>
                item.IsCompensating == true && item.StepName == "InvalidCompensationEventStep3");

            persistedState.SagaInfo.History.ShouldContain(item =>
                item.IsCompensating == false && item.StepName == "InvalidCompensationEventStep3");
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
            await Assert.ThrowsAsync<TestSagaException>(async () =>
            {
                ISagaData sagaData = await sagaCoordinator.
                    Send(startEvent);
            });

            // then
            ISagaData persistedState = await sagaPersistance.Get(correlationID);
            persistedState.ShouldBeNull();
        }

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
            await Assert.ThrowsAsync<TestSagaException>(async () =>
            {
                // when
                ISagaData sagaData = await sagaCoordinator.
                    Send(startEvent);
            });
        }

        [Fact]
        public async Task WHEN_sagaThrowsErrorOnUpdate_THEN_sagaShouldBeInValidState()
        {
            // given
            ISagaData sagaData = await sagaCoordinator.
                Send(new ValidCreatedEvent());

            // when
            await Assert.ThrowsAsync<TestSagaException>(async () =>
            {
                await sagaCoordinator.Send(new InvalidUpdateEvent()
                {
                    CorrelationID = sagaData.CorrelationID
                });
            });

            // then
            InvalidSagaData persistedState = (InvalidSagaData)await sagaPersistance.
                Get(sagaData.CorrelationID);

            persistedState.ShouldNotBeNull();
            persistedState.SagaState.CurrentStep.ShouldBe(null);
            persistedState.SagaState.CurrentState.ShouldBe(nameof(StateCreated));
            persistedState.SagaState.CurrentError.ShouldNotBeNull();
            persistedState.CorrelationID.ShouldBe(sagaData.CorrelationID);

            persistedState.SagaInfo.History.ShouldContain(item =>
                item.IsCompensating == true && item.StepName == "InvalidUpdateEvent1");

            persistedState.SagaInfo.History.ShouldContain(item =>
                item.IsCompensating == false && item.StepName == "InvalidUpdateEvent1");

            persistedState.SagaInfo.History.ShouldContain(item =>
                item.IsCompensating == true && item.StepName == "InvalidUpdateEvent2");

            persistedState.SagaInfo.History.ShouldContain(item =>
                item.IsCompensating == false && item.StepName == "InvalidUpdateEvent2");

            persistedState.SagaInfo.History.ShouldContain(item =>
                item.IsCompensating == true && item.StepName == "InvalidUpdateEvent3");

            persistedState.SagaInfo.History.ShouldContain(item =>
                item.IsCompensating == false && item.StepName == "InvalidUpdateEvent3");
        }

        [Fact]
        public async Task WHEN_sendValidStateToSagaWithError_THEN_errorShouldBeNull()
        {
            // given
            ISagaData sagaData = await sagaCoordinator.
                Send(new ValidCreatedEvent());

            await Assert.ThrowsAsync<TestCompensationException>(async () =>
            {
                await sagaCoordinator.Send(new InvalidCompensationEvent()
                {
                    CorrelationID = sagaData.CorrelationID
                });
            });

            // then
            InvalidSagaData persistedState2 = (InvalidSagaData)await sagaPersistance.
                Get(sagaData.CorrelationID);

            // when
            await sagaCoordinator.Send(new ValidUpdateEvent()
            {
                CorrelationID = sagaData.CorrelationID
            });

            // then
            InvalidSagaData persistedState = (InvalidSagaData)await sagaPersistance.
                Get(sagaData.CorrelationID);

            persistedState.ShouldNotBeNull();
            persistedState.SagaState.CurrentStep.ShouldBe(null);
            persistedState.SagaState.CurrentState.ShouldBe(nameof(StateUpdated));
            persistedState.SagaState.CurrentError.ShouldBeNull();
            persistedState.CorrelationID.ShouldBe(sagaData.CorrelationID);
        }

        #region Arrange

        private ISagaCoordinator sagaCoordinator;
        private ISagaPersistance sagaPersistance;
        private ISagaRegistrator sagaRegistrator;
        private IServiceProvider serviceProvider;

        public SyncAndInvalidSagaTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddTheSaga(cfg =>
            {
#if SQL_SERVER
                cfg.UseSqlServer(new SqlServerOptions()
                {
                    ConnectionString = "data source=lab16;initial catalog=ziarno;uid=dba;pwd=sql;"
                });
#endif
            });

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