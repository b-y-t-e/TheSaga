using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.ExecutionContext;

namespace TheSaga.Tests.SagaTests.SyncAndValid.Activities
{
    internal class SendMessageToTheManagerEvent : ISagaActivity<OrderData>
    {
        public Task Compensate(IExecutionContext<OrderData> context)
        {
            return Task.CompletedTask;
        }

        public Task Execute(IExecutionContext<OrderData> context)
        {
            return Task.CompletedTask;
        }
    }
}
