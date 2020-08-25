using System;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Builders;

namespace TheSaga.Tests.Sagas.OrderTestSaga.Activities
{
    internal class WyslijEmailDoKlienta : ISagaActivity<OrderState>
    {
        public async Task Compensate(IInstanceContext<OrderState> context)
        {
            context.State.Logs.Add($"{nameof(WyslijEmailDoKlienta)} compensation");
        }

        public async Task Execute(IInstanceContext<OrderState> context)
        {
            context.State.Logs.Add(nameof(WyslijEmailDoKlienta));
        }
    }
}
