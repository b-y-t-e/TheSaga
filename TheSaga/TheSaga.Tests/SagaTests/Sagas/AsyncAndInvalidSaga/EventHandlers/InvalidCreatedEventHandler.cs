using System.Threading.Tasks;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Tests.SagaTests.Sagas.AsyncAndInvalidSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.AsyncAndInvalidSaga.EventHandlers
{
    public class InvalidCreatedEventHandler : ISagaEventHandler<InvalidSagaData, InvalidCreatedEvent>
    {
        private readonly ISagaCoordinator sagaCoordinator;

        public InvalidCreatedEventHandler(ISagaCoordinator sagaCoordinator)
        {
            this.sagaCoordinator = sagaCoordinator;
        }

        public Task Compensate(IExecutionContext<InvalidSagaData> context, InvalidCreatedEvent @event)
        {
            return Task.CompletedTask;
        }

        public Task Execute(IExecutionContext<InvalidSagaData> context, InvalidCreatedEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}