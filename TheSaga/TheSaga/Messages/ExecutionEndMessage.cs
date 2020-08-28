using TheSaga.MessageBus;
using TheSaga.Models;

namespace TheSaga.Messages
{
    internal class ExecutionEndMessage : IInternalMessage
    {
        public ISaga Saga { get; set; }

        public ExecutionEndMessage(ISaga saga)
        {
            Saga = saga;
        }
    }
}