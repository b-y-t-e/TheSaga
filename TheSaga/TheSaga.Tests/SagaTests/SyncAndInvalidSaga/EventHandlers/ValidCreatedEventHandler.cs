using System.Threading.Tasks;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Tests.SagaTests.SyncAndInvalidSaga.Events;

namespace TheSaga.Tests.SagaTests.SyncAndInvalidSaga.EventHandlers
{
    public class ValidCreatedEventHandler : ISagaEventHandler<SyncAndInvalidSagaData, ValidCreatedEvent>
    {
        private readonly ISagaCoordinator sagaCoordinator;

        public ValidCreatedEventHandler(ISagaCoordinator sagaCoordinator)
        {
            this.sagaCoordinator = sagaCoordinator;
        }

        public Task Compensate(IExecutionContext<SyncAndInvalidSagaData> context, ValidCreatedEvent @event)
        {
            return Task.CompletedTask;
        }

        public Task Execute(IExecutionContext<SyncAndInvalidSagaData> context, ValidCreatedEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}
