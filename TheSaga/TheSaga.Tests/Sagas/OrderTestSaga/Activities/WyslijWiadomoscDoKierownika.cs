using System;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Builders;

namespace TheSaga.Tests.Sagas.OrderTestSaga.Activities
{
    internal class WyslijWiadomoscDoKierownika : ISagaActivity<OrderState>
    {
        public Task Compensate(IInstanceContext<OrderState> context)
        {
            context.State.Logi.Add($"{nameof(WyslijWiadomoscDoKierownika)} compensation");
            return Task.CompletedTask;
        }

        public Task Execute(IInstanceContext<OrderState> context)
        {
            context.State.Logi.Add($"{nameof(WyslijWiadomoscDoKierownika)}");
            return Task.CompletedTask;
        }
    }
}
