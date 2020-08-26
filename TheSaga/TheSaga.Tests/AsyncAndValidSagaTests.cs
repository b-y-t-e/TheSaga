using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Threading.Tasks;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.Persistance;
using TheSaga.Persistance.SqlServer;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.Registrator;
using TheSaga.SagaStates;
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

            ISagaData sagaData = await sagaCoordinator.
                Send(startEvent);

            // when
            await sagaCoordinator.
                WaitForState<SagaFinishState>(sagaData.CorrelationID);

            // then
            AsyncData persistedState = (AsyncData)await sagaPersistance.
                Get(sagaData.CorrelationID);

            persistedState.ShouldNotBeNull();
            persistedState.SagaState.CurrentStep.ShouldBe(new SagaFinishState().Name);
            persistedState.SagaState.CurrentState.ShouldBe(new SagaFinishState().Name);
            persistedState.CorrelationID.ShouldBe(sagaData.CorrelationID);
            persistedState.SagaInfo.History.ShouldContain(step => step.StepName == "CreatedEventStep0" && !step.IsCompensating && step.HasSucceeded);
            persistedState.SagaInfo.History.ShouldContain(step => step.StepName == "CreatedEventStep1" && !step.IsCompensating && step.HasSucceeded);
            persistedState.SagaInfo.History.ShouldContain(step => step.StepName == "CreatedEventStep2" && !step.IsCompensating && step.HasSucceeded);
            persistedState.SagaInfo.History.Count.ShouldBe(4);
        }

        [Fact]
        public async Task WHEN_runSagaAsynchronous_THEN_sagaShouldBeInIntermediateState()
        {
            // given
            IEvent startEvent = new CreatedEvent();

            // when
            ISagaData sagaData = await sagaCoordinator.
                Send(startEvent);

            // then
            AsyncData persistedState = (AsyncData)await sagaPersistance.
                Get(sagaData.CorrelationID);

            persistedState.ShouldNotBeNull();
            persistedState.SagaState.CurrentStep.ShouldStartWith("CreatedEventStep");
            persistedState.SagaState.CurrentState.ShouldBe(new SagaStartState().Name);
            persistedState.CorrelationID.ShouldBe(sagaData.CorrelationID);
            persistedState.SagaInfo.History.ShouldContain(step => step.StepName == "CreatedEventStep0" && !step.IsCompensating);
            persistedState.SagaInfo.History.Count.ShouldBe(1);
        }

        #region Arrange

        private ISagaCoordinator sagaCoordinator;
        private ISagaPersistance sagaPersistance;
        private ISagaRegistrator sagaRegistrator;
        private IServiceProvider serviceProvider;

        public AsyncAndValidSagaTests()
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