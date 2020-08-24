using System;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Builders;
using TheSaga.Tests.Sagas.OrderTestSaga.Events;

namespace TheSaga.Tests.Sagas.OrderTestSaga.EventHandlers
{
    public class UtworzoneHandler : IEventHandler<OrderState, Utworzone>
    {
        public UtworzoneHandler()
        {

        }

        public Task Compensate(IEventContext<OrderState, Utworzone> context)
        {
            context.State.Logi.Add($"{nameof(UtworzoneHandler)} compensation");
            return Task.CompletedTask;
        }

        public Task Execute(IEventContext<OrderState, Utworzone> context)
        {
            context.State.Logi.Add(nameof(UtworzoneHandler));
            return Task.CompletedTask;
        }
    }
}