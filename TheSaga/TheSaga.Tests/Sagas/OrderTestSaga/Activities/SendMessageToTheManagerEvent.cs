using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Execution.Context;

namespace TheSaga.Tests.Sagas.OrderTestSaga.Activities
{
    internal class SendMessageToTheManagerEvent : ISagaActivity<OrderState>
    {
        public Task Compensate(IExecutionContext<OrderState> context)
        {
            context.State.Logs.Add($"{nameof(SendMessageToTheManagerEvent)} compensation");
            return Task.CompletedTask;
        }

        public Task Execute(IExecutionContext<OrderState> context)
        {
            context.State.Logs.Add($"{nameof(SendMessageToTheManagerEvent)}");
            return Task.CompletedTask;
        }
    }
}