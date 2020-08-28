using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Models.Context;

namespace TheSaga.Tests.Sagas.SyncAndValid.Activities
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