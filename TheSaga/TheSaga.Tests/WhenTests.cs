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
    public class WhenTests
    {
        [Fact]
        public async Task WHEN_eventIsSend_THEN_sagaShouldMoveToValidState()
        {
            // given
            ISagaData sagaData = await sagaCoordinator.
                Send(new OrderCreatedEvent());

            // when
            await sagaCoordinator.
               Send(new ToAlternative1Event() { CorrelationID = sagaData.CorrelationID });

            // then
            ISagaData persistedData = await sagaPersistance.
                Get(sagaData.CorrelationID);

            persistedData.ShouldNotBeNull();
            persistedData.SagaState.CurrentStep.ShouldBe(null);
            persistedData.SagaState.CurrentState.ShouldBe(nameof(StateAlternative1));
            persistedData.CorrelationID.ShouldBe(sagaData.CorrelationID);
        }

        [Fact]
        public async Task WHEN_otherEventIsSend_THEN_sagaShouldMoveToOtherValidState()
        {
            // given
            ISagaData sagaData = await sagaCoordinator.
                Send(new OrderCreatedEvent());

            // when
            await sagaCoordinator.
               Send(new ToAlternative2Event() { CorrelationID = sagaData.CorrelationID });

            // then
            ISagaData persistedData = await sagaPersistance.
                Get(sagaData.CorrelationID);

            persistedData.ShouldNotBeNull();
            persistedData.SagaState.CurrentStep.ShouldBe(null);
            persistedData.SagaState.CurrentState.ShouldBe(nameof(StateAlternative2));
            persistedData.CorrelationID.ShouldBe(sagaData.CorrelationID);
        }

        #region Arrange

        private ISagaCoordinator sagaCoordinator;
        private ISagaPersistance sagaPersistance;
        private ISagaRegistrator sagaRegistrator;
        private IServiceProvider serviceProvider;

        public WhenTests()
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