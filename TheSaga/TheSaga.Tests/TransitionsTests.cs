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
using TheSaga.Tests.Sagas.TransitionsSaga;
using TheSaga.Tests.Sagas.TransitionsSaga.Events;
using TheSaga.Tests.Sagas.TransitionsSaga.States;
using Xunit;

namespace TheSaga.Tests
{
    public class TransitionsTests
    {
        [Fact]
        public async Task WHEN_eventIsSend_THEN_sagaShouldMoveToValidState()
        {
            // given
            ISaga saga = await sagaCoordinator.
                Send(new CreateEvent());

            // when
            await Assert.ThrowsAsync<Exception>(() =>
               sagaCoordinator.
                   Send(new UpdateEvent() { ID = saga.Data.ID }));

            // then
            ISaga persistedSaga = await sagaPersistance.
                Get(saga.Data.ID);

            persistedSaga.ShouldNotBeNull();
            persistedSaga.State.CurrentStep.ShouldBe(null);
            persistedSaga.State.CurrentState.ShouldBe(nameof(Init));
            persistedSaga.Data.ID.ShouldBe(saga.Data.ID);
        }

        #region Arrange

        private ISagaCoordinator sagaCoordinator;
        private ISagaPersistance sagaPersistance;
        private ISagaRegistrator sagaRegistrator;
        private IServiceProvider serviceProvider;

        public TransitionsTests()
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
                new TransitionsSagaDefinition().GetModel(serviceProvider));
        }

        #endregion Arrange
    }
}