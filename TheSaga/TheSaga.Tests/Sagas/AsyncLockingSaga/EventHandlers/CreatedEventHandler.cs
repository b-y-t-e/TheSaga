using System.Threading.Tasks;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.Models.Context;
using TheSaga.Tests.Sagas.AsyncLockingSaga.Events;

namespace TheSaga.Tests.Sagas.AsyncLockingSaga.EventHandlers
{
    public class CreatedEventHandler : IEventHandler<AsyncData, CreatedEvent>
    {
        private readonly ISagaCoordinator sagaCoordinator;

        public CreatedEventHandler(ISagaCoordinator sagaCoordinator)
        {
            this.sagaCoordinator = sagaCoordinator;
        }

        public Task Compensate(IExecutionContext<AsyncData> context, CreatedEvent @event)
        {
            return Task.CompletedTask;
        }

        public Task Execute(IExecutionContext<AsyncData> context, CreatedEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}