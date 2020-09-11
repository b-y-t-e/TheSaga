using TheSaga.Events;
using TheSaga.MessageBus;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.ModelsSaga.Actions.Interfaces;
using TheSaga.ModelsSaga.Interfaces;
using TheSaga.ModelsSaga.Steps.Interfaces;
using TheSaga.ValueObjects;

namespace TheSaga.Commands
{
    internal class ExecuteStepCommand 
    {
        //public ISagaEvent Event;
        public ISagaModel Model;
        public ISaga Saga;
        public ISagaAction SagaAction;
        public ISagaStep SagaStep;
    }
}
