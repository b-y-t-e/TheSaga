using System;
using System.Threading.Tasks;
using TheSaga.Builders;
using TheSaga.Events;
using TheSaga.Execution.Context;
using TheSaga.SagaStates;
using TheSaga.States;

namespace TheSaga.Events
{
    public interface IEventHandler<TSagaState, TEvent> 
        where TSagaState : ISagaState
        where TEvent : IEvent
    {
        Task Execute(IEventContext<TSagaState, TEvent> context);

        Task Compensate(IEventContext<TSagaState, TEvent> context);
    }
}