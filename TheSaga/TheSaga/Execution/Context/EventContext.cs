using TheSaga.Events;
using TheSaga.SagaStates;

namespace TheSaga.Execution.Context
{
    public class EventContext<TSagaState, TEvent> : IEventContext<TSagaState, TEvent>
        where TSagaState : ISagaState
        where TEvent : IEvent
    {
        public TEvent Event { get; set; }
        public TSagaState State { get; set; }
    }
}