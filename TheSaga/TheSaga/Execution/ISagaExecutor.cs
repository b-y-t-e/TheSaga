using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Execution.Actions;
using TheSaga.SagaStates;

namespace TheSaga.Execution
{
    internal interface ISagaExecutor<TSagaState> : ISagaExecutor
        where TSagaState : ISagaState
    { 
    
    }

    internal interface ISagaExecutor
    {
        Task<ISagaState> Handle(Guid correlationID, IEvent @event, IsExecutionAsync async);
    }
}