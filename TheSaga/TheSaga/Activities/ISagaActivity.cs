using System.Threading.Tasks;
using TheSaga.Execution.Context;
using TheSaga.SagaStates;

namespace TheSaga.Activities
{
    public interface ISagaActivity<TSagaState>
        where TSagaState : ISagaState
    {
        Task Compensate(IExecutionContext<TSagaState> context);

        Task Execute(IExecutionContext<TSagaState> context);
    }
}