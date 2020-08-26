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
            ISagaState newSagaState = await sagaCoordinator.
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
            OrderState persistedState = (OrderState)await sagaPersistance.
                Get(newSagaState.CorrelationID);

            persistedState.ShouldNotBeNull();
            persistedState.SagaState.SagaCurrentState.ShouldBe(nameof(StateCreated));
            persistedState.SagaState.SagaCurrentStep.ShouldBe(null);
            persistedState.SagaInfo.SagaHistory.ShouldContain(step => step.StepName == "OrderCreatedEventStep1" && !step.IsCompensating && step.HasSucceeded);
            persistedState.SagaInfo.SagaHistory.Count.ShouldBe(3);
        }

        [Fact]
        public async Task WHEN_someNextEvent_THEN_sagaShouldBeInValidState()
        {
            // given
            ISagaState newSagaState = await sagaCoordinator.
                Send(new OrderCreatedEvent());

            IEvent skompletowanoEvent = new OrderCompletedEvent()
            {
                CorrelationID = newSagaState.CorrelationID
            };

            // then
            await sagaCoordinator.
                Send(skompletowanoEvent);

            // then
            OrderState persistedState = (OrderState)await sagaPersistance.
                Get(newSagaState.CorrelationID);

            persistedState.ShouldNotBeNull();
            persistedState.SagaState.SagaCurrentState.ShouldBe(nameof(StateCompleted));
            persistedState.SagaState.SagaCurrentStep.ShouldBe(null);
            persistedState.SagaInfo.SagaHistory.ShouldContain(step => step.StepName == "OrderCreatedEventStep0" && !step.IsCompensating && step.HasSucceeded);
            persistedState.SagaInfo.SagaHistory.ShouldContain(step => step.StepName == "OrderCreatedEventStep1" && !step.IsCompensating && step.HasSucceeded);
            persistedState.SagaInfo.SagaHistory.ShouldContain(step => step.StepName == "OrderCompletedEventStep1" && !step.IsCompensating && step.HasSucceeded);
            persistedState.SagaInfo.SagaHistory.ShouldContain(step => step.StepName == "email" && !step.IsCompensating && step.HasSucceeded);
            persistedState.SagaInfo.SagaHistory.ShouldContain(step => step.StepName == "SendMessageToTheManagerEventStep" && !step.IsCompensating && step.HasSucceeded);
            persistedState.SagaInfo.SagaHistory.ShouldContain(step => step.StepName == "OrderCourierEventStep" && !step.IsCompensating && step.HasSucceeded);
            persistedState.SagaInfo.SagaHistory.Count.ShouldBe(9);
        }

        [Fact]
        public async Task WHEN_startEvent_THEN_sagaShouldBeCreated()
        {
            // given
            IEvent startEvent = new OrderCreatedEvent();

            // when
            ISagaState sagaState = await sagaCoordinator.
                Send(startEvent);

            // then
            OrderState persistedState = (OrderState)await sagaPersistance.
                Get(sagaState.CorrelationID);

            persistedState.ShouldNotBeNull();
            persistedState.SagaState.SagaCurrentStep.ShouldBe(null);
            persistedState.SagaState.SagaCurrentState.ShouldBe(nameof(StateCreated));
            persistedState.CorrelationID.ShouldBe(sagaState.CorrelationID);
            persistedState.SagaInfo.SagaHistory.ShouldContain(step => step.StepName == "OrderCreatedEventStep1" && !step.IsCompensating && step.HasSucceeded);
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