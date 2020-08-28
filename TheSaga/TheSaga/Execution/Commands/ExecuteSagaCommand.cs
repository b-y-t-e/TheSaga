using System;
using TheSaga.Events;
using TheSaga.SagaModels;
using TheSaga.ValueObjects;

namespace TheSaga.Execution.Commands
{
    internal class ExecuteSagaCommand
    {
        public SagaID ID;
        public IEvent Event;
        public AsyncExecution Async;
        public ISagaModel Model;
    }
}