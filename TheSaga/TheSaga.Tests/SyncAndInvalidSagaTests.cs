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
            ISaga saga = await sagaCoordinator.
                Send(new ValidCreatedEvent());

            // when
            await Assert.ThrowsAsync<TestCompensationException>(async () =>
            {
                await sagaCoordinator.Send(new InvalidCompensationEvent()
                {
                    ID = saga.Data.ID
                });
            });

            // then
            ISaga persistedSaga = await sagaPersistance.
                Get(saga.Data.ID);

            persistedSaga.ShouldNotBeNull();
            persistedSaga.State.CurrentStep.ShouldBe(null);
            persistedSaga.State.CurrentState.ShouldBe(nameof(StateCreated));
            persistedSaga.State.CurrentError.ShouldNotBeNull();
            persistedSaga.Data.ID.ShouldBe(saga.Data.ID);

            persistedSaga.State.History.ShouldContain(item =>
                item.CompensationData != null && item.StepName == "InvalidCompensationEventStep1");

            persistedSaga.State.History.ShouldContain(item =>
                item.CompensationData != null && item.StepName == "InvalidCompensationEventStep2");

            persistedSaga.State.History.ShouldContain(item =>
                item.CompensationData != null && item.StepName == "InvalidCompensationEventStep3");
        }

        [Fact]
        public async Task WHEN_sagaThrowsErrorOnStart_THEN_sagaShouldNotExists()
        {
            // given
            Guid id = Guid.NewGuid();
            IEvent startEvent = new InvalidCreatedEvent()
            {
                ID = id
            };

            // when
            await Assert.ThrowsAsync<TestSagaException>(async () =>
            {
                ISaga saga = await sagaCoordinator.
                    Send(startEvent);
            });

            // then
            ISaga persistedSaga = await sagaPersistance.Get(id);
            persistedSaga.ShouldBeNull();
        }

        [Fact]
        public async Task WHEN_sagaThrowsErrorOnStart_THEN_sagaStepExceptionIsThrown()
        {
            // given
            Guid id = Guid.NewGuid();
            IEvent startEvent = new InvalidCreatedEvent()
            {
                ID = id
            };

            // then
            await Assert.ThrowsAsync<TestSagaException>(async () =>
            {
                // when
                ISaga saga = await sagaCoordinator.
                    Send(startEvent);
            });
        }

        [Fact]
        public async Task WHEN_sagaThrowsErrorOnUpdate_THEN_sagaShouldBeInValidState()
        {
            // given
            ISaga saga = await sagaCoordinator.
                Send(new ValidCreatedEvent());

            // when
            await Assert.ThrowsAsync<TestSagaException>(async () =>
            {
                await sagaCoordinator.Send(new InvalidUpdateEvent()
                {
                    ID = saga.Data.ID
                });
            });

            // then
            ISaga persistedSaga = await sagaPersistance.
                Get(saga.Data.ID);

            persistedSaga.ShouldNotBeNull();
            persistedSaga.State.CurrentStep.ShouldBe(null);
            persistedSaga.State.CurrentState.ShouldBe(nameof(StateCreated));
            persistedSaga.State.CurrentError.ShouldNotBeNull();
            persistedSaga.Data.ID.ShouldBe(saga.Data.ID);

            persistedSaga.State.History.ShouldContain(item =>
                item.CompensationData != null && item.StepName == "InvalidUpdateEvent1");

            persistedSaga.State.History.ShouldContain(item =>
                item.CompensationData != null && item.StepName == "InvalidUpdateEvent2");

            persistedSaga.State.History.ShouldContain(item =>
                item.CompensationData != null && item.StepName == "InvalidUpdateEvent3");

        }

        [Fact]
        public async Task WHEN_sendValidStateToSagaWithError_THEN_errorShouldBeNull()
        {
            // given
            ISaga saga = await sagaCoordinator.
                Send(new ValidCreatedEvent());

            await Assert.ThrowsAsync<TestCompensationException>(async () =>
            {
                await sagaCoordinator.Send(new InvalidCompensationEvent()
                {
                    ID = saga.Data.ID
                });
            });

            // when
            await sagaCoordinator.Send(new ValidUpdateEvent()
            {
                ID = saga.Data.ID
            });

            // then
            ISaga persistedSaga = await sagaPersistance.
                Get(saga.Data.ID);

            persistedSaga.ShouldNotBeNull();
            persistedSaga.State.CurrentStep.ShouldBe(null);
            persistedSaga.State.CurrentState.ShouldBe(nameof(StateUpdated));
            persistedSaga.State.CurrentError.ShouldBeNull();
            persistedSaga.Data.ID.ShouldBe(saga.Data.ID);
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