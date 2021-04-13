using TheSaga.MessageBus.Interfaces;
using TheSaga.Models.Interfaces;

namespace TheSaga.Messages
{
    public class SagaAfterRetrivedMessage : IInternalMessage
    {
        public ISaga Saga;
        public SagaAfterRetrivedMessage(ISaga saga)
        {
            Saga = saga;
        }
    }
}
