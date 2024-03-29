using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Locking;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.Persistance;
using TheSaga.Tests.SagaTests.AsyncLockingSaga.Events;
using Xunit;
using TheSaga.Persistance.SqlServer;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.Locking.DistributedLock;
using TheSaga.Locking.DistributedLock.Options;

namespace TheSaga.Tests.SagaTests.AsyncLockingSaga
{
    public class SagaLockingTests
    {
        [Fact]
        public async Task WHEN_afterAsynchronousSagaRun_THEN_sagaShouldNotBeLocked()
        {
            // given
            ISaga saga = await sagaCoordinator.
                Publish(new CreatedEvent());

            // when
            await sagaCoordinator.
                WaitForIdle(saga.Data.ID);

            // then
            (await sagaLocking.
                IsAcquired(saga.Data.ID)).
                ShouldBeFalse();
        }

        [Fact]
        public async Task WHEN_runSagaAsynchronous_THEN_sagaShouldBeLocked()
        {
            // given
            ISagaEvent startEvent = new CreatedEvent();

            // when
            ISaga saga = await sagaCoordinator.
                Publish(startEvent);

            // then
            (await sagaLocking.
                IsAcquired(saga.Data.ID)).
                ShouldBeTrue();
        }

        [Fact]
        public async Task WHEN_sendingMultipleEventToRunningSaga_THEN_shouldThrowSagaIsBusyException()
        {
            // given
            ISaga saga = await sagaCoordinator.
                Publish(new CreatedEvent());
            await sagaCoordinator.
                WaitForIdle(saga.Data.ID);

            // when
            await sagaCoordinator.
                Publish(new UpdatedEvent() { ID = saga.Data.ID });

            // then
            await Assert.ThrowsAsync<SagaIsBusyException>(async () =>
            {
                await sagaCoordinator.
                    Publish(new UpdatedEvent() { ID = saga.Data.ID });
            });
        }

        #region Arrange

        private ISagaCoordinator sagaCoordinator;
        private ISagaPersistance sagaPersistance;
        private ISagaLocking sagaLocking;

        public SagaLockingTests()
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

            sagaLocking = serviceProvider.
                GetRequiredService<ISagaLocking>();
        }

        #endregion Arrange
    }
}
