using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Threading.Tasks;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.Locking.DistributedLock;
using TheSaga.Locking.DistributedLock.Options;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.Persistance.SqlServer;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.States;
using TheSaga.Tests.SagaTests.Sagas.AsyncAndValid.Events;
using Xunit;

namespace TheSaga.Tests.SagaTests
{
    public class AsyncAndValidSagaTests
    {
        [Fact]
        public async Task WHEN_afterAsynchronousSagaRun_THEN_sagaShouldBeCompleted()
        {
            // given
            ISagaEvent startEvent = new CreatedEvent();

            ISaga saga = await sagaCoordinator.
                Publish(startEvent);

            // when
            await sagaCoordinator.
                WaitForIdle(saga.Data.ID);

            // then
            ISaga persistedSaga = await sagaPersistance.
                Get(saga.Data.ID);

            persistedSaga.ShouldNotBeNull();
            persistedSaga.ExecutionState.CurrentStep.ShouldBe(new SagaFinishState().Name);
            persistedSaga.ExecutionState.CurrentState.ShouldBe(new SagaFinishState().Name);
            persistedSaga.Data.ID.ShouldBe(saga.Data.ID);
            persistedSaga.ExecutionState.History.ShouldContain(step => step.StepName == "CreatedEventStep0" && step.CompensationData == null && step.HasSucceeded());
            persistedSaga.ExecutionState.History.ShouldContain(step => step.StepName == "CreatedEventStep1" && step.CompensationData == null && step.HasSucceeded());
            persistedSaga.ExecutionState.History.ShouldContain(step => step.StepName == "CreatedEventStep2" && step.CompensationData == null && step.HasSucceeded());
            persistedSaga.ExecutionState.History.Count.ShouldBe(4);
        }

        [Fact]
        public async Task WHEN_runSagaAsynchronous_THEN_sagaShouldBeInIntermediateState()
        {
            // given
            ISagaEvent startEvent = new CreatedEvent();

            // when
            ISaga saga = await sagaCoordinator.
                Publish(startEvent);

            // then
            ISaga persistedSaga = await sagaPersistance.
                Get(saga.Data.ID);

            persistedSaga.ShouldNotBeNull();
            persistedSaga.ExecutionState.CurrentStep.ShouldStartWith("CreatedEventStep");
            persistedSaga.ExecutionState.CurrentState.ShouldBe(new SagaStartState().Name);
            persistedSaga.Data.ID.ShouldBe(saga.Data.ID);
            persistedSaga.ExecutionState.History.ShouldContain(step => step.StepName == "CreatedEventStep0" && step.CompensationData == null);
        }

        #region Arrange

        private ISagaCoordinator sagaCoordinator;
        private ISagaPersistance sagaPersistance;

        public AsyncAndValidSagaTests()
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