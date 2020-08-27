using System;
using TheSaga.Events;
using TheSaga.Execution.Actions;
using TheSaga.Models;
using TheSaga.SagaStates;

namespace TheSaga.Execution.Commands
{
    internal class ExecuteActionCommand<TSagaData> 
        where TSagaData : ISagaData
    {
        public IsExecutionAsync Async;
        public IEvent Event;
        public SagaID ID;
        public ISagaModel<TSagaData> Model;
    }
}