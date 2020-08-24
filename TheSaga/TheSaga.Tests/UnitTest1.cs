using System;
using System.Threading;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Builders;
using TheSaga.Coordinators;
using TheSaga.Executors;
using TheSaga.Interfaces;
using TheSaga.Models;
using TheSaga.Registrator;
using TheSaga.Persistance;
using TheSaga.States;
using Xunit;
using Xunit.Sdk;
using TheSaga.Tests.Sagas.OrderTestSaga;

namespace TheSaga.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var sagaModel = new OrderSagaDefinition().GetModel();

            ISagaPersistance sagaPersistance = new InMemorySagaPersistance();
            ISagaRegistrator sagaRegistrator = new SagaRegistrator(sagaPersistance);
            sagaRegistrator.Register(sagaModel);

            ISagaCoordinator sagaCoordinator = new SagaCoordinator(sagaRegistrator);

            sagaCoordinator.Send(new Utworzone());
        }
    }

}
