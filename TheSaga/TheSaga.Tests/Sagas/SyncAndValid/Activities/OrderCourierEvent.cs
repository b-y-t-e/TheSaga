using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Execution.Context;

namespace TheSaga.Tests.Sagas.SyncAndValid.Activities
{
    internal class OrderCourierEvent : ISagaActivity<OrderData>
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