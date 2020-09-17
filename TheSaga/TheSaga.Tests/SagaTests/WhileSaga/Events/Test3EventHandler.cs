using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;

namespace TheSaga.Tests.SagaTests.WhileSaga.Events
{
    public class Test3EventHandler : ISagaEventHandler<WhileSagaData, Test3Event>
    {
        public async Task Compensate(IExecutionContext<WhileSagaData> context, Test3Event @event)
        {

        }

        public async Task Execute(IExecutionContext<WhileSagaData> context, Test3Event @event)
        {
            context.Data.Counter = @event.Condition;
        }
    }
}
