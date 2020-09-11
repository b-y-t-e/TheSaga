using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;

namespace TheSaga.Tests.SagaTests.IfElseIfElseSaga.Events
{
    public class Test3EventHandler : ISagaEventHandler<IfElseIfElseSagaData, Test3Event>
    {
        public async Task Compensate(IExecutionContext<IfElseIfElseSagaData> context, Test3Event @event)
        {

        }

        public async Task Execute(IExecutionContext<IfElseIfElseSagaData> context, Test3Event @event)
        {
            context.Data.Condition = @event.Condition;
        }
    }
}