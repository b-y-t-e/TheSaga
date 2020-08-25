using TheSaga.Events;
using TheSaga.SagaStates;

namespace TheSaga.Execution.Context
{
    public interface IEventContext<TSagaState, TEvent> : IEventContext
        where TSagaState : ISagaState
       where TEvent : IEvent
    {
        public TEvent Event { get; set; }
        public TSagaState State { get; set; }
    }

    public interface IEventContext
    {
    }
}