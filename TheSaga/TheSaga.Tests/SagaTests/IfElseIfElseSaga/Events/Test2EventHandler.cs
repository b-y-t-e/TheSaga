using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;

namespace TheSaga.Tests.SagaTests.IfElseIfElseSaga.Events
{
    public class Test2EventHandler : ISagaEventHandler<IfElseIfElseSagaData, Test2Event>
    {
        public async Task Compensate(IExecutionContext<IfElseIfElseSagaData> context, Test2Event @event)
        {

        }

        public async Task Execute(IExecutionContext<IfElseIfElseSagaData> context, Test2Event @event)
        {
            context.Data.Condition = @event.Condition;
        }
    }
}