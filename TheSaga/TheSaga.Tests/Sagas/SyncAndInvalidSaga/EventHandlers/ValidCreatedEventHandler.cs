using System.Threading.Tasks;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.Execution.Context;
using TheSaga.Tests.Sagas.SyncAndInvalidSaga.Events;

namespace TheSaga.Tests.Sagas.SyncAndInvalidSaga.EventHandlers
{
    public class ValidCreatedEventHandler : IEventHandler<InvalidSagaState, ValidCreatedEvent>
    {
        private readonly ISagaCoordinator sagaCoordinator;

        public ValidCreatedEventHandler(ISagaCoordinator sagaCoordinator)
        {
            this.sagaCoordinator = sagaCoordinator;
        }

        public Task Compensate(IEventContext<InvalidSagaState, ValidCreatedEvent> context)
        {
            return Task.CompletedTask;
        }

        public Task Execute(IEventContext<InvalidSagaState, ValidCreatedEvent> context)
        {
            return Task.CompletedTask;
        }
    }
}