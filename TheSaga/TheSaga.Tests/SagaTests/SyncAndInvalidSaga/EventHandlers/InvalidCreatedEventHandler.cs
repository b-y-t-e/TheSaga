using System.Threading.Tasks;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Tests.SagaTests.Sagas.SyncAndInvalidSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.SyncAndInvalidSaga.EventHandlers
{
    public class InvalidCreatedEventHandler : ISagaEventHandler<SyncAndInvalidSagaData, InvalidCreatedEvent>
    {
        private readonly ISagaCoordinator sagaCoordinator;

        public InvalidCreatedEventHandler(ISagaCoordinator sagaCoordinator)
        {
            this.sagaCoordinator = sagaCoordinator;
        }

        public Task Compensate(IExecutionContext<SyncAndInvalidSagaData> context, InvalidCreatedEvent @event)
        {
            return Task.CompletedTask;
        }

        public Task Execute(IExecutionContext<SyncAndInvalidSagaData> context, InvalidCreatedEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}