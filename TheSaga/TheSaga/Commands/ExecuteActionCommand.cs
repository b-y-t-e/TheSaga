using TheSaga.Events;
using TheSaga.SagaModels;
using TheSaga.ValueObjects;

namespace TheSaga.Commands
{
    internal class ExecuteActionCommand
    {
        public AsyncExecution Async;
        public IEvent Event;
        public SagaID ID;
        public ISagaModel Model;
    }
}