using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Handlers.Events;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.Activities
{
    public interface ISagaActivity<TSagaData>
        where TSagaData : ISagaData
    {
        Task Compensate(IExecutionContext<TSagaData> context);

        Task Execute(IExecutionContext<TSagaData> context);
    }

}
