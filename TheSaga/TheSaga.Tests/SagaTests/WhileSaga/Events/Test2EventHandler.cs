using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;

namespace TheSaga.Tests.SagaTests.WhileSaga.Events
{
    public class Test2EventHandler : ISagaEventHandler<WhileSagaData, Test2Event>
    {
        public async Task Compensate(IExecutionContext<WhileSagaData> context, Test2Event @event)
        {

        }

        public async Task Execute(IExecutionContext<WhileSagaData> context, Test2Event @event)
        {
            context.Data.Counter = @event.Counter;
        }
    }
}
