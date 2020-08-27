using System.Threading.Tasks;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.Execution.Context;
using TheSaga.Tests.Sagas.AsyncLockingSaga.Events;

namespace TheSaga.Tests.Sagas.AsyncLockingSaga
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