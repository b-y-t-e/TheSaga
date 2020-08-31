using System.Threading.Tasks;
using TheSaga.ExecutionContext;
using TheSaga.Models;

namespace TheSaga.SagaModels.Steps.Delegates
{
    public delegate Task ThenActionDelegate<TSagaData>(IExecutionContext<TSagaData> context)
        where TSagaData : ISagaData;
}