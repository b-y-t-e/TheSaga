﻿using TheSaga.Events;
using TheSaga.MessageBus;
using TheSaga.Models;
using TheSaga.SagaModels;
using TheSaga.SagaModels.Actions;
using TheSaga.SagaModels.Steps;
using TheSaga.ValueObjects;

namespace TheSaga.Commands
{
    internal class ExecuteStepCommand 
    {
        public IEvent Event;
        public ISagaModel Model;
        public ISaga Saga;
        public ISagaAction SagaAction;
        public ISagaStep SagaStep;
    }
}