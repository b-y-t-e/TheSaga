using TheSaga.Events;
using TheSaga.SagaStates;
using TheSaga.States;

namespace TheSaga.Execution.Context
{
    public class EventContext<TSagaState, TEvent> : IEventContext<TSagaState, TEvent>
        where TSagaState : ISagaState
        where TEvent : IEvent
    {
        public TSagaState State { get; set; }
        public TEvent Event { get; set; }
    }
}