using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TheSaga.Coordinators;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.Tests.SagaTests.Sagas.SyncAndValid.Events;
using TheSaga.Tests.SagaTests.Sagas.SyncAndValid.States;
using Xunit;

namespace TheSaga.Tests.SagaTests
{
    public class WhenTests
    {
        [Fact]
        public async Task WHEN_eventIsSend_THEN_sagaShouldMoveToValidState()
        {
            // given
            ISaga saga = await sagaCoordinator.
                Publish(new OrderCreatedEvent());

            // when
            await sagaCoordinator.
               Publish(new ToAlternative1Event() { ID = saga.Data.ID });

            // then
            ISaga persistedSaga = await sagaPersistance.
                Get(saga.Data.ID);

            persistedSaga.ShouldNotBeNull();
            persistedSaga.State.CurrentStep.ShouldBe(null);
            persistedSaga.State.CurrentState.ShouldBe(nameof(StateAlternative1));
            persistedSaga.Data.ID.ShouldBe(saga.Data.ID);
        }

        [Fact]
        public async Task WHEN_otherEventIsSend_THEN_sagaShouldMoveToOtherValidState()
        {
            // given
            ISaga saga = await sagaCoordinator.
                Publish(new OrderCreatedEvent());

            // when
            await sagaCoordinator.
               Publish(new ToAlternative2Event() { ID = saga.Data.ID });

            // then
            ISaga persistedSaga = await sagaPersistance.
                Get(saga.Data.ID);

            persistedSaga.ShouldNotBeNull();
            persistedSaga.State.CurrentStep.ShouldBe(null);
            persistedSaga.State.CurrentState.ShouldBe(nameof(StateAlternative2));
            persistedSaga.Data.ID.ShouldBe(saga.Data.ID);
        }

        #region Arrange

        private ISagaCoordinator sagaCoordinator;
        private ISagaPersistance sagaPersistance;

        public WhenTests()
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