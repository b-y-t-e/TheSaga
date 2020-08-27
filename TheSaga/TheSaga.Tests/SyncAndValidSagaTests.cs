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
using TheSaga.Tests.Sagas.SyncAndValid;
using TheSaga.Tests.Sagas.SyncAndValid.Events;
using TheSaga.Tests.Sagas.SyncAndValid.States;
using Xunit;

namespace TheSaga.Tests
{
    public class SyncAndValidSagaTests
    {
        [Fact]
        public async Task WHEN_invalidEvent_THEN_sagaShouldIgnoreThatEvent()
        {
            // given
            ISagaData newSagaState = await sagaCoordinator.
                Send(new OrderCreatedEvent());

            IEvent invalidEvent = new OrderSendEvent()
            {
                CorrelationID = newSagaState.CorrelationID
            };

            // then
            await Assert.ThrowsAsync<SagaInvalidEventForStateException>(() =>
                // when
                sagaCoordinator.
                    Send(invalidEvent)
            );

            // then
            OrderData persistedData = (OrderData)await sagaPersistance.
                Get(newSagaState.CorrelationID);

            persistedData.ShouldNotBeNull();
            persistedData.SagaState.CurrentState.ShouldBe(nameof(StateCreated));
            persistedData.SagaState.CurrentStep.ShouldBe(null);
            persistedData.SagaInfo.History.ShouldContain(step => step.StepName == "OrderCreatedEventStep1" && !step.IsCompensating && step.HasSucceeded);
            persistedData.SagaInfo.History.Count.ShouldBe(3);
        }

        [Fact]
        public async Task WHEN_someNextEvent_THEN_sagaShouldBeInValidState()
        {
            // given
            ISagaData newSagaState = await sagaCoordinator.
                Send(new OrderCreatedEvent());

            IEvent skompletowanoEvent = new OrderCompletedEvent()
            {
                CorrelationID = newSagaState.CorrelationID
            };

            // then
            await sagaCoordinator.
                Send(skompletowanoEvent);

            // then
            OrderData persistedData = (OrderData)await sagaPersistance.
                Get(newSagaState.CorrelationID);

            persistedData.ShouldNotBeNull();
            persistedData.SagaState.CurrentState.ShouldBe(nameof(StateCompleted));
            persistedData.SagaState.CurrentStep.ShouldBe(null);
            persistedData.SagaInfo.History.ShouldContain(step => step.StepName == "OrderCreatedEventStep0" && !step.IsCompensating && step.HasSucceeded);
            persistedData.SagaInfo.History.ShouldContain(step => step.StepName == "OrderCreatedEventStep1" && !step.IsCompensating && step.HasSucceeded);
            persistedData.SagaInfo.History.ShouldContain(step => step.StepName == "OrderCompletedEventStep1" && !step.IsCompensating && step.HasSucceeded);
            persistedData.SagaInfo.History.ShouldContain(step => step.StepName == "email" && !step.IsCompensating && step.HasSucceeded);
            persistedData.SagaInfo.History.ShouldContain(step => step.StepName == "SendMessageToTheManagerEventStep" && !step.IsCompensating && step.HasSucceeded);
            persistedData.SagaInfo.History.ShouldContain(step => step.StepName == "OrderCourierEventStep" && !step.IsCompensating && step.HasSucceeded);
            persistedData.SagaInfo.History.Count.ShouldBe(9);
        }

        [Fact]
        public async Task WHEN_startEvent_THEN_sagaShouldBeCreated()
        {
            // given
            IEvent startEvent = new OrderCreatedEvent();

            // when
            ISagaData sagaData = await sagaCoordinator.
                Send(startEvent);

            // then
            OrderData persistedData = (OrderData)await sagaPersistance.
                Get(sagaData.CorrelationID);

            persistedData.ShouldNotBeNull();
            persistedData.SagaState.CurrentStep.ShouldBe(null);
            persistedData.SagaState.CurrentState.ShouldBe(nameof(StateCreated));
            persistedData.CorrelationID.ShouldBe(sagaData.CorrelationID);
            persistedData.SagaInfo.History.ShouldContain(step => step.StepName == "OrderCreatedEventStep1" && !step.IsCompensating && step.HasSucceeded);
        }

        #region Arrange

        private ISagaCoordinator sagaCoordinator;
        private ISagaPersistance sagaPersistance;
        private ISagaRegistrator sagaRegistrator;
        private IServiceProvider serviceProvider;

        public SyncAndValidSagaTests()
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
                new OrderSagaDefinition().GetModel(serviceProvider));
        }

        #endregion Arrange
    }
}