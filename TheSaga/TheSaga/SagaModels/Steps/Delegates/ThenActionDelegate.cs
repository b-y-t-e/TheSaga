using System.Threading.Tasks;
using TheSaga.Models;
using TheSaga.Models.Context;

namespace TheSaga.SagaModels.Steps.Delegates
{
    public delegate Task ThenActionDelegate<TSagaData>(IExecutionContext<TSagaData> context)
        where TSagaData : ISagaData;
}