using System;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Builders;
using TheSaga.Execution.Context;

namespace TheSaga.Tests.Sagas.OrderTestSaga.Activities
{
    internal class OrderCourierEvent : ISagaActivity<OrderState>
    {
        public Task Compensate(IExecutionContext<OrderState> context)
        {
            context.State.Logs.Add($"{nameof(OrderCourierEvent)} compensation");
            return Task.CompletedTask;
        }

        public Task Execute(IExecutionContext<OrderState> context)
        {
            context.State.Logs.Add($"{nameof(OrderCourierEvent)}");
            return Task.CompletedTask;
        }
    }
}
