using TheSaga.Messages.MessageBus;
using TheSaga.Models;

namespace TheSaga.Messages
{
    internal class ExecutionStartMessage : IInternalMessage
    {
        public ISaga Saga;
        public ExecutionStartMessage(ISaga saga)
        {
            Saga = saga;
        }
    }
}