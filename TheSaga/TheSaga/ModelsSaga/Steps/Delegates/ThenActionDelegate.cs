using System.Threading.Tasks;
using TheSaga.ExecutionContext;
using TheSaga.Models.Interfaces;

namespace TheSaga.ModelsSaga.Steps.Delegates
{
    public delegate Task ThenActionDelegate<TSagaData>(IExecutionContext<TSagaData> context)
        where TSagaData : ISagaData;
}
