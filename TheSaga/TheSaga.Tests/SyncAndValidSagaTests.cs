using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Threading.Tasks;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.Persistance.SqlServer;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.Registrator;
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
            ISaga newSagaState = await sagaCoordinator.
                Publish(new OrderCreatedEvent());

            IEvent invalidEvent = new OrderSendEvent()
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
            persistedSaga.State.CurrentState.ShouldBe(nameof(StateCreated));
            persistedSaga.State.CurrentStep.ShouldBe(null);
            persistedSaga.State.History.ShouldNotContain(step => step.StepName == "OrderCreatedEventStep1" && step.CompensationData == null && step.HasSucceeded());
            persistedSaga.State.History.Count.ShouldBe(0);
        }

        [Fact]
        public async Task WHEN_someNextEvent_THEN_sagaShouldBeInValidState()
        {
            // given
            ISaga newSagaState = await sagaCoordinator.
            Publish(new OrderCreatedEvent());

            IEvent skompletowanoEvent = new OrderCompletedEvent()
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
            persistedSaga.State.CurrentState.ShouldBe(nameof(StateCompleted));
            persistedSaga.State.CurrentStep.ShouldBe(null);
            persistedSaga.State.History.ShouldNotContain(step => step.StepName == "OrderCreatedEventStep0" && step.CompensationData == null && step.HasSucceeded());
            persistedSaga.State.History.ShouldNotContain(step => step.StepName == "OrderCreatedEventStep1" && step.CompensationData == null && step.HasSucceeded());
            persistedSaga.State.History.ShouldContain(step => step.StepName == "OrderCompletedEventStep1" && step.CompensationData == null && step.HasSucceeded());
            persistedSaga.State.History.ShouldContain(step => step.StepName == "email" && step.CompensationData == null && step.HasSucceeded());
            persistedSaga.State.History.ShouldContain(step => step.StepName == "SendMessageToTheManagerEventStep" && step.CompensationData == null && step.HasSucceeded());
            persistedSaga.State.History.ShouldContain(step => step.StepName == "OrderCourierEventStep" && step.CompensationData == null && step.HasSucceeded());
            persistedSaga.State.History.Count.ShouldBe(6);
        }

        [Fact]
        public async Task WHEN_startEvent_THEN_sagaShouldBeCreated()
        {
            // given
            IEvent startEvent = new OrderCreatedEvent();

            // when
            ISaga saga = await sagaCoordinator.
                Publish(startEvent);

            // then
            ISaga persistedSaga = await sagaPersistance.
                Get(saga.Data.ID);

            persistedSaga.ShouldNotBeNull();
            persistedSaga.State.CurrentStep.ShouldBe(null);
            persistedSaga.State.CurrentState.ShouldBe(nameof(StateCreated));
            persistedSaga.Data.ID.ShouldBe(saga.Data.ID);
            persistedSaga.State.History.ShouldContain(step => step.StepName == "OrderCreatedEventStep1" && step.CompensationData == null && step.HasSucceeded());
        }

        #region Arrange

        private ISagaCoordinator sagaCoordinator;
        private ISagaPersistance sagaPersistance;

        public SyncAndValidSagaTests()
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