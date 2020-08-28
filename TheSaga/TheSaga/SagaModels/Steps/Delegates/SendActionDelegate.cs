using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Models;
using TheSaga.Models.Context;

namespace TheSaga.SagaModels.Steps.Delegates
{
    public delegate Task SendActionDelegate<TSagaData, TEvent>(IExecutionContext<TSagaData> context, TEvent @event)
        where TSagaData : ISagaData
        where TEvent : IEvent;
}