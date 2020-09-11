using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;

namespace TheSaga.Tests.SagaTests.IfElseSaga.Events
{
    public class Test2EventHandler : ISagaEventHandler<IfElseSagaData, Test2Event>
    {
        public async Task Compensate(IExecutionContext<IfElseSagaData> context, Test2Event @event)
        {

        }

        public async Task Execute(IExecutionContext<IfElseSagaData> context, Test2Event @event)
        {
            context.Data.Condition = @event.Condition;
        }
    }
}
