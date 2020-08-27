using System;
using TheSaga.Execution.Actions;
using TheSaga.SagaStates;

namespace TheSaga.InternalMessages
{
    internal class SagaExecutionEndMessage : IInternalMessage
    {
        public ISaga Saga { get; set; }

        public SagaExecutionEndMessage(ISaga saga)
        {
            Saga = saga;
        }
    }
}