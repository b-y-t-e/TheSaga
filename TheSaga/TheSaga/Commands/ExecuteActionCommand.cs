using TheSaga.Events;
using TheSaga.Models;
using TheSaga.SagaModels;
using TheSaga.ValueObjects;

namespace TheSaga.Commands
{
    internal class ExecuteActionCommand
    {
        public AsyncExecution Async;
        public IEvent Event;
        // public SagaID ID;
        public ISagaModel Model;

        public ISaga Saga { get; internal set; }
    }
}