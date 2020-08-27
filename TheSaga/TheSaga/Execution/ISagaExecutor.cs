using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Execution.Actions;
using TheSaga.SagaStates;

namespace TheSaga.Execution
{
    internal interface ISagaExecutor<TSagaData> : ISagaExecutor
        where TSagaData : ISagaData
    { 
    
    }

    internal interface ISagaExecutor
    {
        Task<ISaga> Handle(Guid id, IEvent @event, IsExecutionAsync async);
    }
}