using System;
using TheSaga.MessageBus;
using TheSaga.MessageBus.Interfaces;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.ValueObjects;

namespace TheSaga.Messages
{
    public class ExecutionEndMessage : IInternalMessage
    {
        public ExecutionEndMessage(ISaga saga, Exception ex)
        {
            Saga = saga;
            Error = ex;
        }

        public ISaga Saga { get; set; }
        public Exception Error { get; set; }
    }
}
