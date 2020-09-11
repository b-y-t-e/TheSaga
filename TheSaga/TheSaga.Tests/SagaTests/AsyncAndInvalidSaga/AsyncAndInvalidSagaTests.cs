using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TheSaga.Coordinators;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.Persistance;
using TheSaga.Tests.SagaTests.AsyncAndInvalidSaga.Events;
using TheSaga.Tests.SagaTests.AsyncAndInvalidSaga.States;
using Xunit;

namespace TheSaga.Tests.SagaTests.AsyncAndInvalidSaga
{
    public class AsyncAndInvalidSagaTests
    {
        [Fact]
        public async Task WHEN_sagaThrowsErrorOnUpdate_THEN_sagaShouldBeInValidState()
        {
            // given
            ISaga saga = await sagaCoordinator.
                Publish(new ValidCreatedEvent());

            await sagaCoordinator.Publish(new InvalidUpdateEvent()
            {
                ID = saga.Data.ID
            });

            // when
            await sagaCoordinator.
                WaitForIdle(saga.Data.ID);

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

        /*
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
        */

        #region Arrange

        private ISagaCoordinator sagaCoordinator;
        private ISagaPersistance sagaPersistance;

        public AsyncAndInvalidSagaTests()
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
