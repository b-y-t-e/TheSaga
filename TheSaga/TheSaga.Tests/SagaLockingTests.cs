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
            ISagaState sagaState = await sagaCoordinator.
                Send(new CreatedEvent());

            // when
            await sagaCoordinator.
                WaitForState<New>(sagaState.CorrelationID);

            // then
            CorrelationIdLock.
                IsAcquired(sagaState.CorrelationID).
                ShouldBeFalse();
        }

        [Fact]
        public async Task WHEN_runSagaAsynchronous_THEN_sagaShouldBeLocked()
        {
            // given
            IEvent startEvent = new CreatedEvent();

            // when
            ISagaState sagaState = await sagaCoordinator.
                Send(startEvent);

            // then
            CorrelationIdLock.
                IsAcquired(sagaState.CorrelationID).
                ShouldBeTrue();
        }

        [Fact]
        public async Task WHEN_sendingMultipleEventToRunningSaga_THEN_shouldThrowSagaIsBusyException()
        {
            // given
            ISagaState sagaState = await sagaCoordinator.
                Send(new CreatedEvent());
            await sagaCoordinator.
                WaitForState<New>(sagaState.CorrelationID);

            // when
            await sagaCoordinator.
                Send(new UpdatedEvent() { CorrelationID = sagaState.CorrelationID });

            // then
            await Assert.ThrowsAsync<SagaIsBusyException>(async () =>
            {
                await sagaCoordinator.
                    Send(new UpdatedEvent() { CorrelationID = sagaState.CorrelationID });
            });
        }

        #region Arrange

        private ISagaCoordinator sagaCoordinator;
        private ISagaPersistance sagaPersistance;
        private ISagaRegistrator sagaRegistrator;
        private IServiceProvider serviceProvider;

        public SagaLockingTests()
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
                new AsyncSagaDefinition().GetModel(serviceProvider));
        }

        #endregion Arrange
    }
}