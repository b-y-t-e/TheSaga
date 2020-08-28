using System;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.Models;
using TheSaga.Models.Context;

namespace TheSaga.Activities
{
    internal class SendMessageExecuteEventHandler<TSagaData, TEvent> : IEventHandler<TSagaData, TEvent>
        where TSagaData : ISagaData
        where TEvent : IEvent
    {
        ISagaCoordinator sagaCoordinator;

        public SendMessageExecuteEventHandler(ISagaCoordinator sagaCoordinator)
        {
            this.sagaCoordinator = sagaCoordinator;
        }

        public async Task Execute(IExecutionContext<TSagaData> context, TEvent @event)
        {
            await sagaCoordinator.Send(@event);
        }

        public async Task Compensate(IExecutionContext<TSagaData> context, TEvent @event)
        {

        }
    }
}