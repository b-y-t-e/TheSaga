using System;
using System.Threading.Tasks;
using TheSaga.Builders;
using TheSaga.Execution.Context;
using TheSaga.SagaStates;
using TheSaga.States;

namespace TheSaga.Activities
{
    public interface ISagaActivity<TSagaState> 
        where TSagaState : ISagaState
    {
        Task Execute(IExecutionContext<TSagaState> context);

        Task Compensate(IExecutionContext<TSagaState> context);
    }
}