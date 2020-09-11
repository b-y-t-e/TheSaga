using TheSaga.MessageBus;
using TheSaga.MessageBus.Interfaces;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.ModelsSaga.Interfaces;

namespace TheSaga.Messages
{
    internal class ExecutionStartMessage : IInternalMessage
    {
        public ISaga Saga;
        public ISagaModel Model;

        public ExecutionStartMessage(ISaga saga, ISagaModel model)
        {
            Saga = saga;
            this.Model = model;
        }
    }
}
