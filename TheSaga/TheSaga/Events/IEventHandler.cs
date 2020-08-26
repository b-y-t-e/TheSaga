using System.Threading.Tasks;
using TheSaga.Execution.Context;
using TheSaga.SagaStates;

namespace TheSaga.Events
{
    public interface IEventHandler<TSagaData, TEvent>
        where TSagaData : ISagaData
        where TEvent : IEvent
    {
        Task Compensate(IEventContext<TSagaData, TEvent> context);

        Task Execute(IEventContext<TSagaData, TEvent> context);
    }
}