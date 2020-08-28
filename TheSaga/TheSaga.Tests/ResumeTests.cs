using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Threading.Tasks;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Locking;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.Persistance.SqlServer;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.Registrator;
using TheSaga.Tests.Sagas.ResumeSaga;
using TheSaga.Tests.Sagas.ResumeSaga.Events;
using Xunit;

namespace TheSaga.Tests
{
    public class ResumeTests
    {
        [Fact]
        public async Task WHEN_sagaIsStopped_THEN_sagaShouldNotBeInIdleState()
        {
            // given
            ResumeSagaSettings.StopSagaExecution = true;
            ISaga saga = await sagaCoordinator.Send(new ResumeSagaCreateEvent());

            // when
            await sagaCoordinator.Send(new ResumeSagaUpdateEvent() { ID = saga.Data.ID });

            // then
            ISaga persistedSaga = await sagaPersistance.Get(saga.Data.ID);
            persistedSaga.IsIdle().ShouldBeFalse();
        }

        [Fact]
        public async Task WHEN_sagaIsStopped_THEN_sagaLockShouldBeAcquired()
        {
            // given
            ResumeSagaSettings.StopSagaExecution = true;
            ISaga saga = await sagaCoordinator.Send(new ResumeSagaCreateEvent());

            // when
            await sagaCoordinator.Send(new ResumeSagaUpdateEvent() { ID = saga.Data.ID });

            // then
            (await sagaLocking.IsAcquired(saga.Data.ID)).
                ShouldBeTrue();
        }

        [Fact]
        public async Task WHEN_stoppedSagaIsResumed_THEN_sagaShouldMoveToValidState()
        {
            // given
            ResumeSagaSettings.StopSagaExecution = true;
            ISaga saga = await sagaCoordinator.Send(new ResumeSagaCreateEvent());
            await sagaCoordinator.Send(new ResumeSagaUpdateEvent() { ID = saga.Data.ID });
            ResumeSagaSettings.StopSagaExecution = false;
            await sagaLocking.Banish(saga.Data.ID);

            // when
            await sagaCoordinator.
                ResumeAll();

            // then
            ISaga persistedSaga = await sagaPersistance.Get(saga.Data.ID);
            persistedSaga.IsIdle().ShouldBeTrue();
        }

        #region Arrange

        private ISagaCoordinator sagaCoordinator;
        private ISagaPersistance sagaPersistance;
        ISagaLocking sagaLocking;

        public ResumeTests()
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