using Microsoft.VisualStudio.TestPlatform.Common.Telemetry;
using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;

namespace TheSaga.Tests.SagaTests.Sagas.IfElseSaga.Events
{
    public class Test1Event : ISagaEvent
    {
        public Guid ID { get; set; }
        public Boolean Condition { get; set; }
    }

    public class Test1EventHandler : ISagaEventHandler<IfElseSagaData, Test1Event>
    {
        public async Task Compensate(IExecutionContext<IfElseSagaData> context, Test1Event @event)
        {

        }

        public async Task Execute(IExecutionContext<IfElseSagaData> context, Test1Event @event)
        {
            context.Data.Condition = @event.Condition;
        }
    }
}