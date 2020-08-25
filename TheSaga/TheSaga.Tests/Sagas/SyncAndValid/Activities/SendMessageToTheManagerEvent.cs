using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Execution.Context;

namespace TheSaga.Tests.Sagas.SyncAndValid.Activities
{
    internal class SendMessageToTheManagerEvent : ISagaActivity<OrderState>
    {
        public Task Compensate(IExecutionContext<OrderState> context)
        {
            return Task.CompletedTask;
        }

        public Task Execute(IExecutionContext<OrderState> context)
        {
            return Task.CompletedTask;
        }
    }
}