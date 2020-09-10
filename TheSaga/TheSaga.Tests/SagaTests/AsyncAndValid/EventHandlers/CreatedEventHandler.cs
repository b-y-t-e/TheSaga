using System.Threading.Tasks;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Tests.SagaTests.Sagas.AsyncAndValid.Events;

namespace TheSaga.Tests.SagaTests.Sagas.AsyncAndValid.EventHandlers
{
    public class CreatedEventHandler : ISagaEventHandler<AsyncData, CreatedEvent>
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