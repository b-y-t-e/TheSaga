using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Threading.Tasks;
using TheSaga.Coordinators;
using TheSaga.Locking.DistributedLock;
using TheSaga.Locking.DistributedLock.Options;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.Persistance.SqlServer;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.Tests.SagaTests.Sagas.SendTests;
using TheSaga.Tests.SagaTests.Sagas.SendTests.Events;
using TheSaga.Tests.SagaTests.Sagas.SendTests.States;
using Xunit;

namespace TheSaga.Tests.SagaTests
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
            orgSaga.ExecutionState.CurrentState.ShouldBe(nameof(AfterInit));
            orgSaga.ExecutionState.CurrentStep.ShouldBe(null);

            createdSaga.ShouldNotBeNull();
            createdSaga.ExecutionState.CurrentState.ShouldBe(nameof(AkternativeInit));
            createdSaga.ExecutionState.CurrentStep.ShouldBe(null);
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