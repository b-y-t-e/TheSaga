using TheSaga.MessageBus.Interfaces;
using TheSaga.Models.Interfaces;

namespace TheSaga.Messages
{
    public class SagaBeforeStoredMessage : IInternalMessage
    {
        public ISaga Saga;
        public SagaBeforeStoredMessage(ISaga saga)
        {
            Saga = saga;
        }
    }
}
