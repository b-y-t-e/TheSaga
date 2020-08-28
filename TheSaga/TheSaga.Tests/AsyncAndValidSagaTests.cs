using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Threading.Tasks;
using TheSaga.Builders;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.Persistance.SqlServer;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.Registrator;
using TheSaga.States;
using TheSaga.Tests.Sagas.AsyncAndValid;
using TheSaga.Tests.Sagas.AsyncAndValid.Events;
using Xunit;

namespace TheSaga.Tests
{
    public class AsyncAndValidSagaTests
    {
        [Fact]
        public async Task WHEN_afterAsynchronousSagaRun_THEN_sagaShouldBeCompleted()
        {
            // given
            IEvent startEvent = new CreatedEvent();

            ISaga saga = await sagaCoordinator.
                Send(startEvent);

            // when
            await sagaCoordinator.
                WaitForState<SagaFinishState>(saga.Data.ID);

            // then
            ISaga persistedSaga = await sagaPersistance.
                Get(saga.Data.ID);

            persistedSaga.ShouldNotBeNull();
            persistedSaga.State.CurrentStep.ShouldBe(new SagaFinishState().Name);
            persistedSaga.State.CurrentState.ShouldBe(new SagaFinishState().Name);
            persistedSaga.Data.ID.ShouldBe(saga.Data.ID);
            persistedSaga.State.History.ShouldContain(step => step.StepName == "CreatedEventStep0" && step.CompensationData == null && step.HasSucceeded());
            persistedSaga.State.History.ShouldContain(step => step.StepName == "CreatedEventStep1" && step.CompensationData == null && step.HasSucceeded());
            persistedSaga.State.History.ShouldContain(step => step.StepName == "CreatedEventStep2" && step.CompensationData == null && step.HasSucceeded());
            persistedSaga.State.History.Count.ShouldBe(4);
        }

        [Fact]
        public async Task WHEN_runSagaAsynchronous_THEN_sagaShouldBeInIntermediateState()
        {
            // given
            IEvent startEvent = new CreatedEvent();

            // when
            ISaga saga = await sagaCoordinator.
                Send(startEvent);

            // then
            ISaga persistedSaga = await sagaPersistance.
                Get(saga.Data.ID);

            persistedSaga.ShouldNotBeNull();
            persistedSaga.State.CurrentStep.ShouldStartWith("CreatedEventStep");
            persistedSaga.State.CurrentState.ShouldBe(new SagaStartState().Name);
            persistedSaga.Data.ID.ShouldBe(saga.Data.ID);
            persistedSaga.State.History.ShouldContain(step => step.StepName == "CreatedEventStep0" && step.CompensationData == null);
            persistedSaga.State.History.Count.ShouldBe(1);
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