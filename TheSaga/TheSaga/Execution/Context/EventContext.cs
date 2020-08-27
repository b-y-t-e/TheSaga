using TheSaga.Events;
using TheSaga.SagaStates;

namespace TheSaga.Execution.Context
{
    public class EventContext<TSagaData, TEvent> : IEventContext<TSagaData, TEvent>
        where TSagaData : ISagaData
        where TEvent : IEvent
    {
        public EventContext(TEvent @event, TSagaData data, SagaInfo info, SagaState state)
        {
            Event = @event;
            Data = data;
            Info = info;
            State = state;
        }

        public TEvent Event { get; set; }
        public TSagaData Data { get; set; }

        public SagaInfo Info { get; set; }

        public SagaState State { get; set; }
    }
}