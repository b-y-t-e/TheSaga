using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Execution.Context;

namespace TheSaga.Tests.Sagas.SyncAndValid.Activities
{
    internal class SendEmailToClientEvent : ISagaActivity<OrderState>
    {
        public async Task Compensate(IExecutionContext<OrderState> context)
        {
        }

        public async Task Execute(IExecutionContext<OrderState> context)
        {
        }
    }
}