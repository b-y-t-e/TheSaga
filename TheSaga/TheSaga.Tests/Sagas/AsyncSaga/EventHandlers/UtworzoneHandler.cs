using System;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Builders;
using TheSaga.Coordinators;
using TheSaga.Tests.Sagas.AsyncSaga.Events;

namespace TheSaga.Tests.Sagas.AsyncSaga.EventHandlers
{
    public class UtworzoneHandler : IEventHandler<AsyncState, Utworzone>
    {
        private readonly ISagaCoordinator sagaCoordinator;

        public UtworzoneHandler(ISagaCoordinator sagaCoordinator)
        {
            this.sagaCoordinator = sagaCoordinator;
        }

        public Task Compensate(IEventContext<AsyncState, Utworzone> context)
        {
            context.State.Logs.Add($"{nameof(UtworzoneHandler)} compensation");
            return Task.CompletedTask;
        }

        public Task Execute(IEventContext<AsyncState, Utworzone> context)
        {
            context.State.Logs.Add(nameof(UtworzoneHandler));
            return Task.CompletedTask;
        }
    }
}