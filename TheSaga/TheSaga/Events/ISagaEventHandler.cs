using System.Threading.Tasks;
using TheSaga.ExecutionContext;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.Events
{
    public interface ISagaEventHandler<TSagaData, TEvent> : ISagaEventHandler
        where TSagaData : ISagaData
        where TEvent : ISagaEvent
    {
        Task Compensate(IExecutionContext<TSagaData> context, TEvent @event);

        Task Execute(IExecutionContext<TSagaData> context, TEvent @event);
    }
}
