using System;
using TheSaga.Execution.Actions;
using TheSaga.SagaStates;

namespace TheSaga.InternalMessages
{
    internal class SagaExecutionStartMessage : IInternalMessage
    {
        public ISaga Saga;
        public SagaExecutionStartMessage(ISaga saga)
        {
            Saga = saga;
        }
    }
}