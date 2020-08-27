using TheSaga.Events;
using TheSaga.SagaStates;

namespace TheSaga.Execution.Context
{
    public interface IEventContext<TSagaData, TEvent> : IEventContext
        where TSagaData : ISagaData
       where TEvent : IEvent
    {
        TEvent Event { get; set; }
        TSagaData Data { get; set; }
        SagaInfo Info { get; }
        SagaState State { get; }
    }

    public interface IEventContext
    {
    }
}