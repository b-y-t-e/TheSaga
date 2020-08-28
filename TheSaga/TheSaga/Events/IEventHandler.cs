using System.Threading.Tasks;
using TheSaga.Models;
using TheSaga.Models.Context;

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