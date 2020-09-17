using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;

namespace TheSaga.Tests.SagaTests.WhileSaga.Events
{
    public class Test1EventHandler : ISagaEventHandler<WhileSagaData, Test1Event>
    {
        public async Task Compensate(IExecutionContext<WhileSagaData> context, Test1Event @event)
        {

        }

        public async Task Execute(IExecutionContext<WhileSagaData> context, Test1Event @event)
        {
            context.Data.Counter = @event.Counter;
        }
    }
}
