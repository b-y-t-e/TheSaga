using System;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Builders;
using TheSaga.Coordinators;
using TheSaga.Tests.Sagas.AsyncSaga.Events;

namespace TheSaga.Tests.Sagas.AsyncSaga.EventHandlers
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
            context.State.Logs.Add($"{nameof(CreatedEventHandler)} compensation");
            return Task.CompletedTask;
        }

        public Task Execute(IEventContext<AsyncState, CreatedEvent> context)
        {
            context.State.Logs.Add(nameof(CreatedEventHandler));
            return Task.CompletedTask;
        }
    }
}