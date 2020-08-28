using System.Threading.Tasks;
using TheSaga.Models;
using TheSaga.Models.Context;

namespace TheSaga.Activities
{
    public interface ISagaActivity<TSagaData>
        where TSagaData : ISagaData
    {
        Task Compensate(IExecutionContext<TSagaData> context);

        Task Execute(IExecutionContext<TSagaData> context);
    }
}