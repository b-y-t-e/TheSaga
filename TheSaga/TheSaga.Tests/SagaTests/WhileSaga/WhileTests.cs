using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TheSaga.Coordinators;
using TheSaga.Locking;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.Persistance;
using TheSaga.Tests.SagaTests.WhileSaga.Classes;
using TheSaga.Tests.SagaTests.WhileSaga.Events;
using Xunit;
using TheSaga.Persistance.SqlServer;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.Locking.DistributedLock;
using TheSaga.Locking.DistributedLock.Options;

namespace TheSaga.Tests.SagaTests.WhileSaga
{
    public class WhileTests
    {
        [Fact]
        public async Task WHEN_conditionIsMet_THEN_shouldDoIf()
        {
            // given
            ISaga saga = await sagaCoordinator.Publish(new CreateWhileSagaEvent());

            // when
            await sagaCoordinator.Publish(new Test1Event() { ID = saga.Data.ID, Counter = 10 });

            // then
            ISaga persistedSaga = await sagaPersistance.Get(saga.Data.ID);
            WhileSagaData data = persistedSaga.Data as WhileSagaData;
            data.Counter.ShouldBe(0);
            data.Value.ShouldBe(100);
            persistedSaga.ExecutionState.History.Count.ShouldBe(
                1 + // event handler
                3 * 10 + // while + steps
                1 + // last while check
                1); // transition to
        }

        #region Arrange

        private ISagaCoordinator sagaCoordinator;
        private ISagaPersistance sagaPersistance;
        private ISagaLocking sagaLocking;

        public WhileTests()
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

            sagaLocking = serviceProvider.
                GetRequiredService<ISagaLocking>();
        }

        #endregion Arrange
    }
}