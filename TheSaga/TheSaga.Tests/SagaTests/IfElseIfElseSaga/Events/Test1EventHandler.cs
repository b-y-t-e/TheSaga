using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;

namespace TheSaga.Tests.SagaTests.IfElseIfElseSaga.Events
{
    public class Test1EventHandler : ISagaEventHandler<IfElseIfElseSagaData, Test1Event>
    {
        public async Task Compensate(IExecutionContext<IfElseIfElseSagaData> context, Test1Event @event)
        {

        }

        public async Task Execute(IExecutionContext<IfElseIfElseSagaData> context, Test1Event @event)
        {
            context.Data.Condition = @event.Condition;
        }
    }
}