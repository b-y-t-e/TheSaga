using TheSaga.MessageBus;
using TheSaga.MessageBus.Interfaces;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

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
