using TheSaga.Events;
using TheSaga.SagaStates;
using TheSaga.States;

namespace TheSaga.Execution.Context
{

    public interface IEventContext<TSagaState, TEvent> : IEventContext
        where TSagaState : ISagaState
       where TEvent : IEvent
    {
        public TSagaState State { get; set; }
        public TEvent Event { get; set; }
    }

    public interface IEventContext
    {
    }
}