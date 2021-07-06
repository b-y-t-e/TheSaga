using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TheSaga.Coordinators;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.Persistance;
using TheSaga.Tests.SagaTests.SendTests.Events;
using TheSaga.Tests.SagaTests.SendTests.States;
using Xunit;
using TheSaga.Persistance.SqlServer;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.Locking.DistributedLock;
using TheSaga.Locking.DistributedLock.Options;

namespace TheSaga.Tests.SagaTests.SendTests
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
                    ConnectionString = "data source=.;initial catalog=sagatest;uid=dba;pwd=sql;"
                });
                cfg.UseDistributedLock(new SqlServerLockingOptions()
                {
                    ConnectionString = "data source=.;initial catalog=sagatest;uid=dba;pwd=sql;"
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
