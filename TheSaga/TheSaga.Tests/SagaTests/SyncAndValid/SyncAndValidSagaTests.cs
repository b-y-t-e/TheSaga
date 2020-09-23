using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.Persistance;
using TheSaga.Tests.SagaTests.SyncAndValid.Events;
using TheSaga.Tests.SagaTests.SyncAndValid.States;
using Xunit;
using TheSaga.Persistance.SqlServer;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.Locking.DistributedLock;
using TheSaga.Locking.DistributedLock.Options;
using TheSaga.ModelsSaga.History;

namespace TheSaga.Tests.SagaTests.SyncAndValid
{
    public class SyncAndValidSagaTests
    {
        [Fact]
        public async Task WHEN_invalidEvent_THEN_sagaShouldIgnoreThatEvent()
        {
            // given
            ISaga newSagaState = await sagaCoordinator.
                Publish(new OrderCreatedEvent());

            ISagaEvent invalidEvent = new OrderSendEvent()
            {
                ID = newSagaState.Data.ID
            };

            // then
            await Assert.ThrowsAsync<SagaInvalidEventForStateException>(() =>
                // when
                sagaCoordinator.
                    Publish(invalidEvent)
            );

            // then
            ISaga persistedSaga = await sagaPersistance.
                Get(newSagaState.Data.ID);

            persistedSaga.ShouldNotBeNull();
            persistedSaga.ExecutionState.CurrentState.ShouldBe(nameof(StateCreated));
            persistedSaga.ExecutionState.CurrentStep.ShouldBe(null);
            // blad nie powinien zmienic stanu sagi
            persistedSaga.ExecutionState.History.ShouldContain(step => step.StepName == "OrderCreatedEventStep1" && step.CompensationData == null && step.HasSucceeded());
            persistedSaga.ExecutionState.History.Count.ShouldBe(3);
        }

        [Fact]
        public async Task WHEN_someNextEvent_THEN_sagaShouldBeInValidState()
        {
            // given
            ISaga newSagaState = await sagaCoordinator.
            Publish(new OrderCreatedEvent());

            ISagaEvent skompletowanoEvent = new OrderCompletedEvent()
            {
                ID = newSagaState.Data.ID
            };

            // then
            await sagaCoordinator.
                Publish(skompletowanoEvent);

            // then
            ISaga persistedSaga = await sagaPersistance.
                Get(newSagaState.Data.ID);

            persistedSaga.ShouldNotBeNull();
            persistedSaga.ExecutionState.CurrentState.ShouldBe(nameof(StateCompleted));
            persistedSaga.ExecutionState.CurrentStep.ShouldBe(null);
            persistedSaga.ExecutionState.History.ShouldNotContain(step => step.StepName == "OrderCreatedEventStep0" && step.CompensationData == null && step.HasSucceeded());
            persistedSaga.ExecutionState.History.ShouldNotContain(step => step.StepName == "OrderCreatedEventStep1" && step.CompensationData == null && step.HasSucceeded());
            persistedSaga.ExecutionState.History.ShouldContain(step => step.StepName == "OrderCompletedEventStep1" && step.CompensationData == null && step.HasSucceeded());
            persistedSaga.ExecutionState.History.ShouldContain(step => step.StepName == "email" && step.CompensationData == null && step.HasSucceeded());
            persistedSaga.ExecutionState.History.ShouldContain(step => step.StepName == "SendMessageToTheManagerEventStep" && step.CompensationData == null && step.HasSucceeded());
            persistedSaga.ExecutionState.History.ShouldContain(step => step.StepName == "OrderCourierEventStep" && step.CompensationData == null && step.HasSucceeded());
            persistedSaga.ExecutionState.History.Count.ShouldBe(6);
        }

        [Fact]
        public async Task WHEN_startEvent_THEN_sagaShouldBeCreated()
        {
            // given
            ISagaEvent startEvent = new OrderCreatedEvent();

            // when
            ISaga saga = await sagaCoordinator.
                Publish(startEvent);

            // then
            ISaga persistedSaga = await sagaPersistance.
                Get(saga.Data.ID);

            persistedSaga.ShouldNotBeNull();
            persistedSaga.ExecutionState.CurrentStep.ShouldBe(null);
            persistedSaga.ExecutionState.CurrentState.ShouldBe(nameof(StateCreated));
            persistedSaga.Data.ID.ShouldBe(saga.Data.ID);
            persistedSaga.ExecutionState.History.ShouldContain(step => step.StepName == "OrderCreatedEventStep1" && step.CompensationData == null && step.HasSucceeded());
        }

        #region Arrange

        private ISagaCoordinator sagaCoordinator;
        private ISagaPersistance sagaPersistance;

        public SyncAndValidSagaTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSaga(cfg =>
            {
                cfg.UseFilePersistance();
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
