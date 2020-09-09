﻿using TheSaga.MessageBus;
using TheSaga.Models;
using TheSaga.SagaModels;

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