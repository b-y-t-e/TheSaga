using System;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Builders;

namespace TheSaga.Tests.Sagas.OrderTestSaga.Activities
{
    internal class ZamowKuriera : ISagaActivity<OrderState>
    {
        public Task Compensate(IInstanceContext<OrderState> context)
        {
            context.State.Logs.Add($"{nameof(ZamowKuriera)} compensation");
            return Task.CompletedTask;
        }

        public Task Execute(IInstanceContext<OrderState> context)
        {
            context.State.Logs.Add($"{nameof(ZamowKuriera)}");
            return Task.CompletedTask;
        }
    }
}
