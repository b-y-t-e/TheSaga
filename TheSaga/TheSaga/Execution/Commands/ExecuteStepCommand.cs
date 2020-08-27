using System;
using TheSaga.Events;
using TheSaga.Execution.Actions;
using TheSaga.Models;
using TheSaga.SagaStates;
using TheSaga.SagaStates.Actions;
using TheSaga.SagaStates.Steps;

namespace TheSaga.Execution.Commands
{
    internal class ExecuteStepCommand<TSagaData> 
        where TSagaData : ISagaData
    {
        public ISaga saga;
        public ISagaAction sagaAction;
        public ISagaStep sagaStep;
        public IEvent @event;
        public IsExecutionAsync @async;
    }
}