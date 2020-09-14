using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.Persistance;
using TheSaga.Tests.SagaTests.SyncAndInvalidSaga.Events;
using TheSaga.Tests.SagaTests.SyncAndInvalidSaga.Exceptions;
using TheSaga.Tests.SagaTests.SyncAndInvalidSaga.States;
using Xunit;
using TheSaga.Persistance.SqlServer;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.Locking.DistributedLock;
using TheSaga.Locking.DistributedLock.Options;

namespace TheSaga.Tests.SagaTests.SyncAndInvalidSaga
{
    public class SyncAndInvalidSagaTests
    {
        [Fact]
        public async Task WHEN_compensationThrowsErrorOnUpdate_THEN_sagaShouldBeInValidState()
        {
            // given
            ISaga saga = await sagaCoordinator.
                Publish(new ValidCreatedEvent());

            // when
            await Assert.ThrowsAsync<TestCompensationException>(async () =>
            {
                await sagaCoordinator.Publish(new InvalidCompensationEvent()
                {
                    ID = saga.Data.ID
                });
            });

            // then
            ISaga persistedSaga = await sagaPersistance.
                Get(saga.Data.ID);

            persistedSaga.ShouldNotBeNull();
            persistedSaga.ExecutionState.CurrentStep.ShouldBe(null);
            persistedSaga.ExecutionState.CurrentState.ShouldBe(nameof(StateCreated));
            persistedSaga.ExecutionState.CurrentError.ShouldNotBeNull();
            persistedSaga.Data.ID.ShouldBe(saga.Data.ID);

            persistedSaga.ExecutionState.History.ShouldContain(item =>
                item.CompensationData != null && item.StepName == "InvalidCompensationEventStep1");

            persistedSaga.ExecutionState.History.ShouldContain(item =>
                item.CompensationData != null && item.StepName == "InvalidCompensationEventStep2");

            persistedSaga.ExecutionState.History.ShouldContain(item =>
                item.CompensationData != null && item.StepName == "InvalidCompensationEventStep3");
        }

        [Fact]
        public async Task WHEN_sagaThrowsErrorOnStart_THEN_sagaShouldNotExists()
        {
            // given
            Guid id = Guid.NewGuid();
            ISagaEvent startEvent = new InvalidCreatedEvent()
            {
                ID = id
            };

            // when
            await Assert.ThrowsAsync<TestSagaException>(async () =>
            {
                ISaga saga = await sagaCoordinator.
                    Publish(startEvent);
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
            ISagaEvent startEvent = new InvalidCreatedEvent()
            {
                ID = id
            };

            // then
            await Assert.ThrowsAsync<TestSagaException>(async () =>
            {
                // when
                ISaga saga = await sagaCoordinator.
                    Publish(startEvent);
            });
        }

        [Fact]
        public async Task WHEN_sagaThrowsErrorOnUpdate_THEN_sagaShouldBeInValidState()
        {
            // given
            ISaga saga = await sagaCoordinator.
                Publish(new ValidCreatedEvent());

            // when
            await Assert.ThrowsAsync<TestSagaException>(async () =>
            {
                await sagaCoordinator.Publish(new InvalidUpdateEvent()
                {
                    ID = saga.Data.ID
                });
            });

            // then
            ISaga persistedSaga = await sagaPersistance.
                Get(saga.Data.ID);

            persistedSaga.ShouldNotBeNull();
            persistedSaga.ExecutionState.CurrentStep.ShouldBe(null);
            persistedSaga.ExecutionState.CurrentState.ShouldBe(nameof(StateCreated));
            persistedSaga.ExecutionState.CurrentError.ShouldNotBeNull();
            persistedSaga.Data.ID.ShouldBe(saga.Data.ID);

            persistedSaga.ExecutionState.History.ShouldContain(item =>
                item.CompensationData != null && item.StepName == "InvalidUpdateEvent1");

            persistedSaga.ExecutionState.History.ShouldContain(item =>
                item.CompensationData != null && item.StepName == "InvalidUpdateEvent2");

            persistedSaga.ExecutionState.History.ShouldContain(item =>
                item.CompensationData != null && item.StepName == "InvalidUpdateEvent3");
        }

        [Fact]
        public async Task WHEN_sendValidStateToSagaWithError_THEN_errorShouldBeNull()
        {
            // given
            ISaga saga = await sagaCoordinator.
                Publish(new ValidCreatedEvent());

            await Assert.ThrowsAsync<TestCompensationException>(async () =>
            {
                await sagaCoordinator.Publish(new InvalidCompensationEvent()
                {
                    ID = saga.Data.ID
                });
            });

            // when
            await sagaCoordinator.Publish(new ValidUpdateEvent()
            {
                ID = saga.Data.ID
            });

            // then
            ISaga persistedSaga = await sagaPersistance.
                Get(saga.Data.ID);

            persistedSaga.ShouldNotBeNull();
            persistedSaga.ExecutionState.CurrentStep.ShouldBe(null);
            persistedSaga.ExecutionState.CurrentState.ShouldBe(nameof(StateUpdated));
            persistedSaga.ExecutionState.CurrentError.ShouldBeNull();
            persistedSaga.Data.ID.ShouldBe(saga.Data.ID);
        }

        #region Arrange

        private ISagaCoordinator sagaCoordinator;
        private ISagaPersistance sagaPersistance;

        public SyncAndInvalidSagaTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSaga(cfg =>
            {
#if SQL_SERVER
                cfg.UseSqlServer(new SqlServerOptions()
                {
                    ConnectionString = "data source=lab16;initial catalog=ziarno;uid=dba;pwd=sql;"
                });
                cfg.UseDistributedLock(new SqlServerLockingOptions()
                {
                    ConnectionString = "data source=lab16;initial catalog=ziarno;uid=dba;pwd=sql;"
                });
#endif
            });

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            sagaPersistance = serviceProvider.
                GetRequiredService<ISagaPersistance>();

            sagaCoordinator = serviceProvider.
                GetRequiredService<ISagaCoordinator>();
        }

        #endregion Arrange
    }
}
