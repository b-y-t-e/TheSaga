using System.Threading.Tasks;
using TheSaga.Models.Context;

namespace TheSaga.Models.Steps
{
    public delegate Task ThenActionDelegate<TSagaData>(IExecutionContext<TSagaData> context)
        where TSagaData : ISagaData;
}