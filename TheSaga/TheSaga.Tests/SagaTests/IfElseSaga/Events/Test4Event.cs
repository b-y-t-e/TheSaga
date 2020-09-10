using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;

namespace TheSaga.Tests.SagaTests.Sagas.IfElseSaga.Events
{
    public class Test4Event : ISagaEvent
    {
        public Guid ID { get; set; }
        public int Condition { get; set; }
    }
    public class Test4EventHandler : ISagaEventHandler<IfElseSagaData, Test4Event>
    {
        public async Task Compensate(IExecutionContext<IfElseSagaData> context, Test4Event @event)
        {

        }

        public async Task Execute(IExecutionContext<IfElseSagaData> context, Test4Event @event)
        {
            context.Data.Condition = @event.Condition;
        }
    }
}