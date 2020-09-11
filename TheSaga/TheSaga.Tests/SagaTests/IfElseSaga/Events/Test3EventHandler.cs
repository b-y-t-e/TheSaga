using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;

namespace TheSaga.Tests.SagaTests.IfElseSaga.Events
{
    public class Test3EventHandler : ISagaEventHandler<IfElseSagaData, Test3Event>
    {
        public async Task Compensate(IExecutionContext<IfElseSagaData> context, Test3Event @event)
        {

        }

        public async Task Execute(IExecutionContext<IfElseSagaData> context, Test3Event @event)
        {
            context.Data.Condition = @event.Condition;
        }
    }
}
