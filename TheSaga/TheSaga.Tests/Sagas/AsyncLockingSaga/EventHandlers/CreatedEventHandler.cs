using System.Threading.Tasks;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.Execution.Context;
using TheSaga.Tests.Sagas.AsyncLockingSaga.Events;

namespace TheSaga.Tests.Sagas.AsyncLockingSaga.EventHandlers
{
    public class CreatedEventHandler : IEventHandler<AsyncState, CreatedEvent>
    {
        private readonly ISagaCoordinator sagaCoordinator;

        public CreatedEventHandler(ISagaCoordinator sagaCoordinator)
        {
            this.sagaCoordinator = sagaCoordinator;
        }

        public Task Compensate(IEventContext<AsyncState, CreatedEvent> context)
        {
            return Task.CompletedTask;
        }

        public Task Execute(IEventContext<AsyncState, CreatedEvent> context)
        {
            return Task.CompletedTask;
        }
    }
}