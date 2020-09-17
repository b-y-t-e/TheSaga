using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models.Interfaces;

namespace TheSaga.ModelsSaga.Steps.Delegates
{
    public delegate Task SendActionAsyncDelegate<TSagaData, TEvent>(IExecutionContext<TSagaData> context, TEvent @event)
        where TSagaData : ISagaData
        where TEvent : ISagaEvent;

    public delegate void SendActionDelegate<TSagaData, TEvent>(IExecutionContext<TSagaData> context, TEvent @event)
        where TSagaData : ISagaData
        where TEvent : ISagaEvent;
}
