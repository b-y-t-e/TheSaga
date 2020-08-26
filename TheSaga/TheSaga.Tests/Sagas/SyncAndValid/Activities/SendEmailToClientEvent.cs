using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Execution.Context;

namespace TheSaga.Tests.Sagas.SyncAndValid.Activities
{
    internal class SendEmailToClientEvent : ISagaActivity<OrderData>
    {
        public async Task Compensate(IExecutionContext<OrderData> context)
        {
        }

        public async Task Execute(IExecutionContext<OrderData> context)
        {
        }
    }
}