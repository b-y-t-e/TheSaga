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
using TheSaga.Tests.Sagas.SendTests;
using TheSaga.Tests.Sagas.SendTests.Events;
using TheSaga.Tests.Sagas.SendTests.States;
using Xunit;

namespace TheSaga.Tests
{
    public class SendTests
    {
        [Fact]
        public async Task WHEN_eventIsSend_THEN_sagaShouldMoveToValidState()
        {
            // given
            ISaga saga = await sagaCoordinator.
                Publish(new SendCreateEvent());

            // when
            await sagaCoordinator.
                Publish(new TestSendActionEvent() { ID = saga.Data.ID });

            // then
            ISaga orgSaga = await sagaPersistance.
                Get(saga.Data.ID);

            SendTestsData data = orgSaga.Data as SendTestsData;

            ISaga createdSaga = await sagaPersistance.
                Get(data.CreatedSagaID);

            orgSaga.ShouldNotBeNull();
            orgSaga.State.CurrentState.ShouldBe(nameof(AfterInit));
            orgSaga.State.CurrentStep.ShouldBe(null);

            createdSaga.ShouldNotBeNull();
            createdSaga.State.CurrentState.ShouldBe(nameof(AkternativeInit));
            createdSaga.State.CurrentStep.ShouldBe(null);
        }

        #region Arrange

        private ISagaCoordinator sagaCoordinator;
        private ISagaPersistance sagaPersistance;

        public SendTests()
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