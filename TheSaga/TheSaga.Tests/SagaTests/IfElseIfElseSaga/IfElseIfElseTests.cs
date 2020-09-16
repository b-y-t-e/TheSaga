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
using TheSaga.Models.Interfaces;
using TheSaga.Persistance;
using TheSaga.Persistance.SqlServer;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.Tests.SagaTests.IfElseIfElseSaga.Classes;
using TheSaga.Tests.SagaTests.IfElseIfElseSaga.Events;
using Xunit;

namespace TheSaga.Tests.SagaTests.IfElseIfElseSaga
{
    public class IfElseIfElseTests
    {
        [Fact]
        public async Task WHEN_conditionIsNotMet_THEN_null()
        {
            // given
            ISaga saga = await sagaCoordinator.Publish(new CreateIfElseSagaEvent());

            // when
            await sagaCoordinator.Publish(new Test4Event() { ID = saga.Data.ID, Condition = 0 });

            // then
            ISaga persistedSaga = await sagaPersistance.Get(saga.Data.ID);
            IfElseIfElseSagaData data = persistedSaga.Data as IfElseIfElseSagaData;
            data.Value1.ShouldBe(100);
        }

        [Fact]
        public async Task WHEN_conditionIsNotMet_THEN_1()
        {
            // given
            ISaga saga = await sagaCoordinator.Publish(new CreateIfElseSagaEvent());

            // when
            await sagaCoordinator.Publish(new Test4Event() { ID = saga.Data.ID, Condition = 1 });

            // then
            ISaga persistedSaga = await sagaPersistance.Get(saga.Data.ID);
            IfElseIfElseSagaData data = persistedSaga.Data as IfElseIfElseSagaData;
            data.Value1.ShouldBe(1);
        }

        [Fact]
        public async Task WHEN_conditionIsNotMet_THEN_2()
        {
            // given
            ISaga saga = await sagaCoordinator.Publish(new CreateIfElseSagaEvent());

            // when
            await sagaCoordinator.Publish(new Test4Event() { ID = saga.Data.ID, Condition = 2 });

            // then
            ISaga persistedSaga = await sagaPersistance.Get(saga.Data.ID);
            IfElseIfElseSagaData data = persistedSaga.Data as IfElseIfElseSagaData;
            data.Value1.ShouldBe(2);
        }

        [Fact]
        public async Task WHEN_conditionIsNotMet_THEN_3()
        {
            // given
            ISaga saga = await sagaCoordinator.Publish(new CreateIfElseSagaEvent());

            // when
            await sagaCoordinator.Publish(new Test4Event() { ID = saga.Data.ID, Condition = 3 });

            // then
            ISaga persistedSaga = await sagaPersistance.Get(saga.Data.ID);
            IfElseIfElseSagaData data = persistedSaga.Data as IfElseIfElseSagaData;
            data.Value1.ShouldBe(3);
        }

        [Fact]
        public async Task WHEN_elseIf_else_THEN_100()
        {
            // given
            ISaga saga = await sagaCoordinator.Publish(new CreateIfElseSagaEvent());

            // when
            await sagaCoordinator.Publish(new Test4Event() { ID = saga.Data.ID, Condition = 4 });

            // then
            ISaga persistedSaga = await sagaPersistance.Get(saga.Data.ID);
            IfElseIfElseSagaData data = persistedSaga.Data as IfElseIfElseSagaData;
            data.Value1.ShouldBe(100);
        }

        [Fact]
        public async Task WHEN_elseIf_if_THEN_4()
        {
            // given
            ISaga saga = await sagaCoordinator.Publish(new CreateIfElseSagaEvent());

            // when
            await sagaCoordinator.Publish(new Test4Event() { ID = saga.Data.ID, Condition = 4, SubCondition = 1 });

            // then
            ISaga persistedSaga = await sagaPersistance.Get(saga.Data.ID);
            IfElseIfElseSagaData data = persistedSaga.Data as IfElseIfElseSagaData;
            data.Value1.ShouldBe(4);
        }

        [Fact]
        public async Task WHEN_conditionIsNotMet_THEN_5()
        {
            // given
            ISaga saga = await sagaCoordinator.Publish(new CreateIfElseSagaEvent());

            // when
            await sagaCoordinator.Publish(new Test4Event() { ID = saga.Data.ID, Condition = 5 });

            // then
            ISaga persistedSaga = await sagaPersistance.Get(saga.Data.ID);
            IfElseIfElseSagaData data = persistedSaga.Data as IfElseIfElseSagaData;
            data.Value1.ShouldBe(5);
        }

        #region Arrange

        private ISagaCoordinator sagaCoordinator;
        private ISagaPersistance sagaPersistance;
        private ISagaLocking sagaLocking;

        public IfElseIfElseTests()
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
