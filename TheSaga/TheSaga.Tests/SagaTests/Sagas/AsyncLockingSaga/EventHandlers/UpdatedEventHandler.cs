using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Tests.SagaTests.Sagas.AsyncLockingSaga.Events;

namespace TheSaga.Tests.SagaTests.Sagas.AsyncLockingSaga.EventHandlers
{    
    public class UpdatedEventHandler : IEventHandler<AsyncData, UpdatedEvent>
    {
        public UpdatedEventHandler()
        {

        }

        public Task Compensate(IExecutionContext<AsyncData> context, UpdatedEvent @event)
        {
            return Task.CompletedTask;
        }

        public Task Execute(IExecutionContext<AsyncData> context, UpdatedEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}