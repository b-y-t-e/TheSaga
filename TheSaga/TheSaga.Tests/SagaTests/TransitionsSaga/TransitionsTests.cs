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
using TheSaga.Tests.SagaTests.Sagas.TransitionsSaga.Events;
using TheSaga.Tests.SagaTests.Sagas.TransitionsSaga.States;
using Xunit;

namespace TheSaga.Tests.SagaTests
{
    public class TransitionsTests
    {
        [Fact]
        public async Task WHEN_eventIsSend_THEN_sagaShouldMoveToValidState()
        {
            // given
            ISaga saga = await sagaCoordinator.
                Publish(new CreateEvent());

            // when
            await Assert.ThrowsAsync<Exception>(() =>
               sagaCoordinator.
                   Publish(new InvalidUpdateEvent() { ID = saga.Data.ID }));

            // then
            ISaga persistedSaga = await sagaPersistance.
                Get(saga.Data.ID);

            persistedSaga.ShouldNotBeNull();
            persistedSaga.ExecutionState.CurrentStep.ShouldBe(null);
            persistedSaga.ExecutionState.CurrentState.ShouldBe(nameof(Init));
            persistedSaga.Data.ID.ShouldBe(saga.Data.ID);
        }

        #region Arrange

        private ISagaCoordinator sagaCoordinator;
        private ISagaPersistance sagaPersistance;

        public TransitionsTests()
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