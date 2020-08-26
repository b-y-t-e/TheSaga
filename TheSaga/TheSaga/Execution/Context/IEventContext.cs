using TheSaga.Events;
using TheSaga.SagaStates;

namespace TheSaga.Execution.Context
{
    public interface IEventContext<TSagaData, TEvent> : IEventContext
        where TSagaData : ISagaData
       where TEvent : IEvent
    {
        public TEvent Event { get; set; }
        public TSagaData State { get; set; }
    }

    public interface IEventContext
    {
    }
}