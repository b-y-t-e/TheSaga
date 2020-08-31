using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Locking;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.Tests.SagaTests.Sagas.AsyncLockingSaga.Events;
using TheSaga.Tests.SagaTests.Sagas.AsyncLockingSaga.States;
using Xunit;

namespace TheSaga.Tests.SagaTests
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
            IEvent startEvent = new CreatedEvent();

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
        ISagaLocking sagaLocking;

        public SagaLockingTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSaga(cfg =>
            {
#if SQL_SERVER
                cfg.UseSqlServer(new SqlServerOptions()
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

            sagaLocking = serviceProvider.
                GetRequiredService<ISagaLocking>();
        }

        #endregion Arrange
    }
}