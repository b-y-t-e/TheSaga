using System;
using TheSaga.Events;
using TheSaga.Models;
using TheSaga.Models.Actions;
using TheSaga.Models.Steps;
using TheSaga.SagaModels;
using TheSaga.ValueObjects;

namespace TheSaga.Execution.Commands
{
    internal class ExecuteStepCommand
    {
        public ISaga Saga;
        public ISagaAction SagaAction;
        public ISagaStep SagaStep;
        public IEvent Event;
        public AsyncExecution Async;
        public ISagaModel Model;       
    }
}