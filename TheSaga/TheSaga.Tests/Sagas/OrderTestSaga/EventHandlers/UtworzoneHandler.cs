using System;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Builders;
using TheSaga.Coordinators;
using TheSaga.Tests.Sagas.OrderTestSaga.Events;

namespace TheSaga.Tests.Sagas.OrderTestSaga.EventHandlers
{
    public class UtworzoneHandler : IEventHandler<OrderState, Utworzone>
    {
        private readonly ISagaCoordinator sagaCoordinator;

        public UtworzoneHandler(ISagaCoordinator sagaCoordinator)
        {
            this.sagaCoordinator = sagaCoordinator;
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