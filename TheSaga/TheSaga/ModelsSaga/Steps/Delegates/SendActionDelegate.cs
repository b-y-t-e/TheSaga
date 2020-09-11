using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models.Interfaces;

namespace TheSaga.ModelsSaga.Steps.Delegates
{
    public delegate Task SendActionDelegate<TSagaData, TEvent>(IExecutionContext<TSagaData> context, TEvent @event)
        where TSagaData : ISagaData
        where TEvent : ISagaEvent;

}
