using System.Threading.Tasks;
using TheSaga.ExecutionContext;
using TheSaga.Models;

namespace TheSaga.Events
{
    public interface IEventHandler<TSagaData, TEvent> : IEventHandler
        where TSagaData : ISagaData
        where TEvent : IEvent
    {
        Task Compensate(IExecutionContext<TSagaData> context, TEvent @event);

        Task Execute(IExecutionContext<TSagaData> context, TEvent @event);
    }

    public interface IEventHandler
    {
    }
}