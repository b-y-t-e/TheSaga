using System.Threading.Tasks;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.Models.Context;
using TheSaga.Tests.Sagas.SyncAndInvalidSaga.Events;

namespace TheSaga.Tests.Sagas.SyncAndInvalidSaga.EventHandlers
{
    public class InvalidCreatedEventHandler : IEventHandler<InvalidSagaData, InvalidCreatedEvent>
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