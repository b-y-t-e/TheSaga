using TheSaga.MessageBus;
using TheSaga.MessageBus.Interfaces;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.ValueObjects;

namespace TheSaga.Messages
{
    internal class ExecutionEndMessage : IInternalMessage
    {
        public ExecutionEndMessage(SagaID sagaId)
        {
            SagaId = sagaId;
        }

        public SagaID SagaId { get; set; }
    }
}
