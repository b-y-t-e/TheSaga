using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Threading.Tasks;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Persistance;
using TheSaga.Persistance.SqlServer;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.Registrator;
using TheSaga.SagaStates;
using TheSaga.Tests.Sagas.AsyncLockingSaga;
using TheSaga.Tests.Sagas.AsyncLockingSaga.Events;
using TheSaga.Tests.Sagas.AsyncLockingSaga.States;
using TheSaga.Utils;
using Xunit;

namespace TheSaga.Tests
{
    public class SagaLockingTests
    {
        [Fact]
        public async Task WHEN_afterAsynchronousSagaRun_THEN_sagaShouldNotBeLocked()
        {
            // given
            ISaga saga = await sagaCoordinator.
                Send(new CreatedEvent());

            // when
            await sagaCoordinator.
                WaitForState<New>(saga.Data.ID);

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
                Send(startEvent);

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
                Send(new CreatedEvent());
            await sagaCoordinator.
                WaitForState<New>(saga.Data.ID);

            // when
            await sagaCoordinator.
                Send(new UpdatedEvent() { ID = saga.Data.ID });

            // then
            await Assert.ThrowsAsync<SagaIsBusyException>(async () =>
            {
                await sagaCoordinator.
                    Send(new UpdatedEvent() { ID = saga.Data.ID });
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