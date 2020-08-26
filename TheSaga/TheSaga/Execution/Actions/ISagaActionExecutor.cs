using System;
using System.Threading.Tasks;
using TheSaga.SagaStates;

namespace TheSaga.Execution.Actions
{
    internal interface ISagaActionExecutor<TSagaState>
        where TSagaState : ISagaState
    {
        Task<ActionExecutionResult> ExecuteAction();
    }
}