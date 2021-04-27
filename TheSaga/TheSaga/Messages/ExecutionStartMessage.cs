using TheSaga.MessageBus;
using TheSaga.MessageBus.Interfaces;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.ModelsSaga.Interfaces;
using TheSaga.ValueObjects;

namespace TheSaga.Messages
{
    public class ExecutionStartMessage : IInternalMessage
    {
        public ISagaModel Model;
        public ISaga Saga;

        public ExecutionStartMessage(ISaga saga, ISagaModel model)
        {
            Saga = saga;
            Model = model;
        }
    }
}
