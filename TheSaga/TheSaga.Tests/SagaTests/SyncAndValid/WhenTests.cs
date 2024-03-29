using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TheSaga.Coordinators;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.Persistance;
using TheSaga.Tests.SagaTests.SyncAndValid.Events;
using TheSaga.Tests.SagaTests.SyncAndValid.States;
using Xunit;
using TheSaga.Persistance.SqlServer;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.Locking.DistributedLock;
using TheSaga.Locking.DistributedLock.Options;

namespace TheSaga.Tests.SagaTests.SyncAndValid
{
    public class WhenTests
    {
       /* [Fact]
        public async Task WHEN_eventIsSend_THEN_2222()
        {
            // given
            ISaga saga = await sagaCoordinator.
                Publish(new OrderCreatedEvent());

            // when
            for (var i = 0; i < 1000; i++)
                Task.Run(() =>
                    sagaCoordinator.
                        Publish(new OrderCompletedEvent() { ID = saga.Data.ID }));

            Thread.Sleep(200000);

            // then
            ISaga persistedSaga = await sagaPersistance.
                Get(saga.Data.ID);

            persistedSaga.ShouldNotBeNull();
            persistedSaga.ExecutionState.CurrentStep.ShouldBe(null);
            persistedSaga.ExecutionState.CurrentState.ShouldBe(nameof(StateAlternative1));
            persistedSaga.Data.ID.ShouldBe(saga.Data.ID);
        }
        */

        [Fact]
        public async Task WHEN_eventIsSend_THEN_sagaShouldMoveToValidState()
        {
            // given
            ISaga saga = await sagaCoordinator.
                Publish(new OrderCreatedEvent());

            // when
            await sagaCoordinator.
               Publish(new ToAlternative1Event() { ID = saga.Data.ID });

            // then
            ISaga persistedSaga = await sagaPersistance.
                Get(saga.Data.ID);

            persistedSaga.ShouldNotBeNull();
            persistedSaga.ExecutionState.CurrentStep.ShouldBe(null);
            persistedSaga.ExecutionState.CurrentState.ShouldBe(nameof(StateAlternative1));
            persistedSaga.Data.ID.ShouldBe(saga.Data.ID);
        }

        [Fact]
        public async Task WHEN_otherEventIsSend_THEN_sagaShouldMoveToOtherValidState()
        {
            // given
            ISaga saga = await sagaCoordinator.
                Publish(new OrderCreatedEvent());

            // when
            await sagaCoordinator.
               Publish(new ToAlternative2Event() { ID = saga.Data.ID });

            // then
            ISaga persistedSaga = await sagaPersistance.
                Get(saga.Data.ID);

            persistedSaga.ShouldNotBeNull();
            persistedSaga.ExecutionState.CurrentStep.ShouldBe(null);
            persistedSaga.ExecutionState.CurrentState.ShouldBe(nameof(StateAlternative2));
            persistedSaga.Data.ID.ShouldBe(saga.Data.ID);
        }

        #region Arrange

        private ISagaCoordinator sagaCoordinator;
        private ISagaPersistance sagaPersistance;

        public WhenTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSaga(cfg =>
            {
#if SQL_SERVER
                cfg.UseSqlServer(new SqlServerOptions()
                {
                    ConnectionString = "data source=.;initial catalog=sagatest;uid=dba;pwd=sql;"
                });
                cfg.UseDistributedLock(new SqlServerLockingOptions()
                {
                    ConnectionString = "data source=.;initial catalog=sagatest;uid=dba;pwd=sql;"
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
