using System.Threading.Tasks;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.Execution.Context;
using TheSaga.Tests.Sagas.SyncAndInvalidSaga.Events;

namespace TheSaga.Tests.Sagas.SyncAndInvalidSaga.EventHandlers
{
    public class InvalidCreatedEventHandler : IEventHandler<InvalidSagaState, InvalidCreatedEvent>
    {
        private readonly ISagaCoordinator sagaCoordinator;

        public InvalidCreatedEventHandler(ISagaCoordinator sagaCoordinator)
        {
            this.sagaCoordinator = sagaCoordinator;
        }

        public Task Compensate(IEventContext<InvalidSagaState, InvalidCreatedEvent> context)
        {
            context.State.Logs.Add($"{nameof(InvalidCreatedEventHandler)} compensation");
            return Task.CompletedTask;
        }

        public Task Execute(IEventContext<InvalidSagaState, InvalidCreatedEvent> context)
        {
            context.State.Logs.Add(nameof(InvalidCreatedEventHandler));
            return Task.CompletedTask;
        }
    }
}