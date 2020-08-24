using System;
using System.Threading.Tasks;
using TheSaga.Builders;
using TheSaga.Interfaces;
using TheSaga.States;

namespace TheSaga.Activities
{
    public interface IEventHandler<TSagaState, TEvent> 
        where TSagaState : ISagaState
        where TEvent : IEvent
    {
        Task Execute(IEventContext<TSagaState, TEvent> context);

        Task Compensate(IEventContext<TSagaState, TEvent> context);
    }
}