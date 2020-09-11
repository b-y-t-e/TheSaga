using System.Threading.Tasks;
using TheSaga.ExecutionContext;
using TheSaga.Models.Interfaces;

namespace TheSaga.ModelsSaga.Steps.Delegates
{
    public delegate Task<bool> IfFuncDelegate<TSagaData>(IExecutionContext<TSagaData> context)
        where TSagaData : ISagaData;
}
