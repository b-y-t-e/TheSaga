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

            ISagaState sagaState = await sagaCoordinator.
                Send(startEvent);

            // when
            await sagaCoordinator.
                WaitForState<SagaFinishState>(sagaState.CorrelationID);

            // then
            AsyncState persistedState = (AsyncState)await sagaPersistance.
                Get(sagaState.CorrelationID);

            persistedState.ShouldNotBeNull();
            persistedState.SagaCurrentStep.ShouldBe(new SagaFinishState().Name);
            persistedState.SagaCurrentState.ShouldBe(new SagaFinishState().Name);
            persistedState.CorrelationID.ShouldBe(sagaState.CorrelationID);
            persistedState.SagaHistory.ShouldContain(step => step.StepName == "CreatedEventStep0" && !step.IsCompensating && step.HasSucceeded);
            persistedState.SagaHistory.ShouldContain(step => step.StepName == "CreatedEventStep1" && !step.IsCompensating && step.HasSucceeded);
            persistedState.SagaHistory.ShouldContain(step => step.StepName == "CreatedEventStep2" && !step.IsCompensating && step.HasSucceeded);
            persistedState.SagaHistory.Count.ShouldBe(4);
        }

        [Fact]
        public async Task WHEN_runSagaAsynchronous_THEN_sagaShouldBeInIntermediateState()
        {
            // given
            IEvent startEvent = new CreatedEvent();

            // when
            ISagaState sagaState = await sagaCoordinator.
                Send(startEvent);

            // then
            AsyncState persistedState = (AsyncState)await sagaPersistance.
                Get(sagaState.CorrelationID);

            persistedState.ShouldNotBeNull();
            persistedState.SagaCurrentStep.ShouldStartWith("CreatedEventStep");
            persistedState.SagaCurrentState.ShouldBe(new SagaStartState().Name);
            persistedState.CorrelationID.ShouldBe(sagaState.CorrelationID);
            persistedState.SagaHistory.ShouldContain(step => step.StepName == "CreatedEventStep0" && !step.IsCompensating);
            persistedState.SagaHistory.Count.ShouldBe(1);
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