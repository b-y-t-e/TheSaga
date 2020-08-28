using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TheSaga.Coordinators;
using TheSaga.Locking;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.Tests.SagaTests.Sagas.ResumeSaga;
using TheSaga.Tests.SagaTests.Sagas.ResumeSaga.Events;
using Xunit;

namespace TheSaga.Tests.SagaTests
{
    public class ResumeTests
    {
        [Fact]
        public async Task WHEN_sagaIsStopped_THEN_sagaShouldNotBeInIdleState()
        {
            // given
            ResumeSagaSettings.StopSagaExecution = true;
            ISaga saga = await sagaCoordinator.Publish(new ResumeSagaCreateEvent());

            // when
            await sagaCoordinator.Publish(new ResumeSagaUpdateEvent() { ID = saga.Data.ID });

            // then
            ISaga persistedSaga = await sagaPersistance.Get(saga.Data.ID);
            persistedSaga.IsIdle().ShouldBeFalse();
        }

        [Fact]
        public async Task WHEN_sagaIsStopped_THEN_sagaLockShouldBeAcquired()
        {
            // given
            ResumeSagaSettings.StopSagaExecution = true;
            ISaga saga = await sagaCoordinator.Publish(new ResumeSagaCreateEvent());

            // when
            await sagaCoordinator.Publish(new ResumeSagaUpdateEvent() { ID = saga.Data.ID });

            // then
            (await sagaLocking.IsAcquired(saga.Data.ID)).
                ShouldBeTrue();
        }

        [Fact]
        public async Task WHEN_stoppedSagaIsResumed_THEN_sagaShouldMoveToValidState()
        {
            // given
            ResumeSagaSettings.StopSagaExecution = true;
            ISaga saga = await sagaCoordinator.Publish(new ResumeSagaCreateEvent());
            await sagaCoordinator.Publish(new ResumeSagaUpdateEvent() { ID = saga.Data.ID });
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