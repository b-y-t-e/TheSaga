using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;

namespace TheSaga.Tests.SagaTests.IfElseIfElseSaga.Events
{
    public class Test4EventHandler : ISagaEventHandler<IfElseIfElseSagaData, Test4Event>
    {
        public async Task Compensate(IExecutionContext<IfElseIfElseSagaData> context, Test4Event @event)
        {

        }

        public async Task Execute(IExecutionContext<IfElseIfElseSagaData> context, Test4Event @event)
        {
            context.Data.Condition = @event.Condition;
            context.Data.SubCondition = @event.SubCondition;
        }
    }
}