using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models;

namespace TheSaga.SagaModels.Steps.Delegates
{
    public delegate Task SendActionDelegate<TSagaData, TEvent>(IExecutionContext<TSagaData> context, TEvent @event)
        where TSagaData : ISagaData
        where TEvent : IEvent;
}