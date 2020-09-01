using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Threading.Tasks;
using TheSaga.Coordinators;
using TheSaga.Locking.DistributedLock;
using TheSaga.Locking.DistributedLock.Options;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.Persistance.SqlServer;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.Tests.SagaTests.Sagas.AsyncAndInvalidSaga.Events;
using TheSaga.Tests.SagaTests.Sagas.AsyncAndInvalidSaga.States;
using Xunit;

namespace TheSaga.Tests.SagaTests
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
            persistedSaga.State.CurrentStep.ShouldBe(null);
            persistedSaga.State.CurrentState.ShouldBe(nameof(StateUpdated));
            persistedSaga.State.CurrentError.ShouldBeNull();
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