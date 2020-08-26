using System;
using System.Threading.Tasks;
using TheSaga.SagaStates;

namespace TheSaga.Execution.Actions
{
    internal interface ISagaActionExecutor<TSagaData>
        where TSagaData : ISagaData
    {
        Task<ActionExecutionResult> ExecuteAction();
    }
}