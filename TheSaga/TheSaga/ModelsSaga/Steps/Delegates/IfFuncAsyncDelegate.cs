using System.Threading.Tasks;
using TheSaga.ExecutionContext;
using TheSaga.Models.Interfaces;

namespace TheSaga.ModelsSaga.Steps.Delegates
{
    public delegate Task<bool> IfFuncAsyncDelegate<TSagaData>(IExecutionContext<TSagaData> context)
        where TSagaData : ISagaData;

    public delegate bool IfFuncDelegate<TSagaData>(IExecutionContext<TSagaData> context)
        where TSagaData : ISagaData;
}
