using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models;

namespace TheSaga.Activities
{
    public interface ISagaActivity<TSagaData>
        where TSagaData : ISagaData
    {
        Task Compensate(IExecutionContext<TSagaData> context);

        Task Execute(IExecutionContext<TSagaData> context);
    }

    public interface IHandlersActivity
    {
        Task Compensate(IExecutionContext<HandlersData> context);

        Task Execute(IExecutionContext<HandlersData> context);
    }
}