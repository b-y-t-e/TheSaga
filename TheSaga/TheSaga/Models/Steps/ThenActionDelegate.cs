using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Models.Context;

namespace TheSaga.Models.Steps
{
    public delegate Task ThenActionDelegate<TSagaData>(IExecutionContext<TSagaData> context)
        where TSagaData : ISagaData;

    public delegate Task<TEvent> SendActionDelegate<TSagaData, TEvent>(IExecutionContext<TSagaData> context, TEvent @event)
        where TSagaData : ISagaData
        where TEvent : IEvent;
}