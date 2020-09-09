using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models;

namespace TheSaga.SagaModels.Steps.Delegates
{
    public delegate Task ThenActionDelegate<TSagaData>(IExecutionContext<TSagaData> context)
        where TSagaData : ISagaData;

    public delegate Task HandlersThenActionDelegate (IExecutionContext<HandlersData> context);
}