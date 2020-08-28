using TheSaga.MessageBus;
using TheSaga.Models;

namespace TheSaga.Messages
{
    internal class ExecutionEndMessage : IInternalMessage
    {
        public ExecutionEndMessage(ISaga saga)
        {
            Saga = saga;
        }

        public ISaga Saga { get; set; }
    }
}