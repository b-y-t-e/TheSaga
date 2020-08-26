using TheSaga.Events;
using TheSaga.SagaStates;

namespace TheSaga.Execution.Context
{
    public class EventContext<TSagaData, TEvent> : IEventContext<TSagaData, TEvent>
        where TSagaData : ISagaData
        where TEvent : IEvent
    {
        public TEvent Event { get; set; }
        public TSagaData State { get; set; }
    }
}