﻿using System.Threading.Tasks;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.Models;
using TheSaga.Models.Context;

namespace TheSaga.EventHandlers
{
    internal class SendMessageCompensateEventHandler<TSagaData, TEvent> : IEventHandler<TSagaData, TEvent>
        where TSagaData : ISagaData
        where TEvent : IEvent
    {
        ISagaCoordinator sagaCoordinator;

        public SendMessageCompensateEventHandler(ISagaCoordinator sagaCoordinator)
        {
            this.sagaCoordinator = sagaCoordinator;
        }

        public async Task Execute(IExecutionContext<TSagaData> context, TEvent @event)
        {
        }

        public async Task Compensate(IExecutionContext<TSagaData> context, TEvent @event)
        {
            await sagaCoordinator.Publish(@event);
        }
    }
}