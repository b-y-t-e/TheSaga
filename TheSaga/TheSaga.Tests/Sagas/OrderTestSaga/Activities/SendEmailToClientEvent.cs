using System;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Builders;

namespace TheSaga.Tests.Sagas.OrderTestSaga.Activities
{
    internal class SendEmailToClientEvent : ISagaActivity<OrderState>
    {
        public async Task Compensate(IInstanceContext<OrderState> context)
        {
            context.State.Logs.Add($"{nameof(SendEmailToClientEvent)} compensation");
        }

        public async Task Execute(IInstanceContext<OrderState> context)
        {
            context.State.Logs.Add(nameof(SendEmailToClientEvent));
        }
    }
}
