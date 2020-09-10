using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using TheSaga.Coordinators;
using TheSaga.Locking;
using TheSaga.Locking.DistributedLock;
using TheSaga.Locking.DistributedLock.Options;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.Persistance.SqlServer;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.Tests.SagaTests.Sagas.IfElseSaga;
using TheSaga.Tests.SagaTests.Sagas.IfElseSaga.Events;
using Xunit;

namespace TheSaga.Tests.SagaTests
{
    public class IfElseTests
    {
        [Fact]
        public async Task WHEN_conditionIsMet_THEN_shouldDoIf()
        {
            // given
            ISaga saga = await sagaCoordinator.Publish(new CreateIfElseSagaEvent());

            // when
            await sagaCoordinator.Publish(new Test1Event() { ID = saga.Data.ID, Condition = true });

            // then
            ISaga persistedSaga = await sagaPersistance.Get(saga.Data.ID);
            IfElseSagaData data = persistedSaga.Data as IfElseSagaData;
            data.Condition.ShouldBeTrue();
            data.Value1.ShouldBeOfType<Test1Event>();
        }

        [Fact]
        public async Task WHEN_conditionIsNotMet_THEN_shouldAvoidIf()
        {
            // given
            ISaga saga = await sagaCoordinator.Publish(new CreateIfElseSagaEvent());

            // when
            await sagaCoordinator.Publish(new Test2Event() { ID = saga.Data.ID });

            // then
            ISaga persistedSaga = await sagaPersistance.Get(saga.Data.ID);
            IfElseSagaData data = persistedSaga.Data as IfElseSagaData;
            data.Condition.ShouldBeFalse();
            data.Value1.ShouldNotBeOfType<Test2Event>();
        }

        [Fact]
        public async Task WHEN_conditionIsMet_THEN_shouldOmitElse()
        {
            // given
            ISaga saga = await sagaCoordinator.Publish(new CreateIfElseSagaEvent());

            // when
            await sagaCoordinator.Publish(new Test3Event() { ID = saga.Data.ID, Condition = true });

            // then
            ISaga persistedSaga = await sagaPersistance.Get(saga.Data.ID);
            IfElseSagaData data = persistedSaga.Data as IfElseSagaData;
            data.Value1.ShouldBeOfType<TrueValue>();
        }

        [Fact]
        public async Task WHEN_conditionIsNotMet_THEN_shouldDoElse()
        {
            // given
            ISaga saga = await sagaCoordinator.Publish(new CreateIfElseSagaEvent());

            // when
            await sagaCoordinator.Publish(new Test3Event() { ID = saga.Data.ID, Condition = false });

            // then
            ISaga persistedSaga = await sagaPersistance.Get(saga.Data.ID);
            IfElseSagaData data = persistedSaga.Data as IfElseSagaData;
            data.Value1.ShouldBeOfType<FalseValue>();
        }

        #region Arrange

        private ISagaCoordinator sagaCoordinator;
        private ISagaPersistance sagaPersistance;
        private ISagaLocking sagaLocking;

        public IfElseTests()
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