using TheSaga.MessageBus;
using TheSaga.MessageBus.Interfaces;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.ModelsSaga.Interfaces;
using TheSaga.ValueObjects;

namespace TheSaga.Messages
{
    internal class ExecutionStartMessage : IInternalMessage
    {
        public SagaID SagaID;
        public ISagaModel Model;

        public ExecutionStartMessage(SagaID saga, ISagaModel model)
        {
            SagaID = saga;
            this.Model = model;
        }
    }
}
