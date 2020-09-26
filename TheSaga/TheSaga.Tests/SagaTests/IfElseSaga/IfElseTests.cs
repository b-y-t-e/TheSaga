using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TheSaga.Coordinators;
using TheSaga.Locking;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.Persistance;
using TheSaga.Tests.SagaTests.IfElseSaga.Classes;
using TheSaga.Tests.SagaTests.IfElseSaga.Events;
using Xunit;
using TheSaga.Persistance.SqlServer;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.Locking.DistributedLock;
using TheSaga.Locking.DistributedLock.Options;

namespace TheSaga.Tests.SagaTests.IfElseSaga
{
    public class IfElseTests
    {
        [Fact]
        public async Task WHEN_conditionIsMet_THEN_shouldDoIf()
        {
            // given
            ISaga saga = await sagaCoordinator.Publish(new CreateIfElseSagaEvent());

            // when
            await sagaCoordinator.Publish(new Test1Event() { ID = saga.Data.ID, Condition = 1 });

            // then
            ISaga persistedSaga = await sagaPersistance.Get(saga.Data.ID);
            IfElseSagaData data = persistedSaga.Data as IfElseSagaData;
            data.Condition.ShouldBe(1);
            data.Value1.ShouldBeOfType<TrueValue>();
        }

        [Fact]
        public async Task WHEN_conditionIsNotMet_THEN_shouldAvoidIf0()
        {
            // given
            ISaga saga = await sagaCoordinator.Publish(new CreateIfElseSagaEvent());

            // when
            await sagaCoordinator.Publish(new Test1Event() { ID = saga.Data.ID, Condition = 0 });

            // then
            ISaga persistedSaga = await sagaPersistance.Get(saga.Data.ID);
            IfElseSagaData data = persistedSaga.Data as IfElseSagaData;
            data.Condition.ShouldBe(0);
            data.Value1.ShouldNotBeOfType<TrueValue>();
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
            data.Condition.ShouldBe(0);
            data.Value1.ShouldNotBeOfType<TrueValue>();
        }

        [Fact]
        public async Task WHEN_conditionIsMet_THEN_shouldDoIfAndOmitElse()
        {
            // given
            ISaga saga = await sagaCoordinator.Publish(new CreateIfElseSagaEvent());

            // when
            await sagaCoordinator.Publish(new Test3Event() { ID = saga.Data.ID, Condition = 1 });

            // then
            ISaga persistedSaga = await sagaPersistance.Get(saga.Data.ID);
            IfElseSagaData data = persistedSaga.Data as IfElseSagaData;
            data.Value1.ShouldBeOfType<TrueValue>();
        }

        [Fact]
        public async Task WHEN_conditionIsNotMet_THEN_shouldOmitIfAndDoElse()
        {
            // given
            ISaga saga = await sagaCoordinator.Publish(new CreateIfElseSagaEvent());

            // when
            await sagaCoordinator.Publish(new Test3Event() { ID = saga.Data.ID, Condition = 0 });

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

            sagaLocking = serviceProvider.
                GetRequiredService<ISagaLocking>();
        }

        #endregion Arrange
    }
}
