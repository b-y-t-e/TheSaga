using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;

namespace TheSaga.Tests.SagaTests.WhileSaga.Events
{
    public class Test4EventHandler : ISagaEventHandler<WhileSagaData, Test4Event>
    {
        public async Task Compensate(IExecutionContext<WhileSagaData> context, Test4Event @event)
        {

        }

        public async Task Execute(IExecutionContext<WhileSagaData> context, Test4Event @event)
        {
            context.Data.Counter = @event.Condition;
        }
    }
}
