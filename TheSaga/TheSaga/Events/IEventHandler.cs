using System.Threading.Tasks;
using TheSaga.Execution.Context;
using TheSaga.SagaStates;

namespace TheSaga.Events
{
    public interface IEventHandler<TSagaState, TEvent>
        where TSagaState : ISagaState
        where TEvent : IEvent
    {
        Task Compensate(IEventContext<TSagaState, TEvent> context);

        Task Execute(IEventContext<TSagaState, TEvent> context);
    }
}