using System;
using System.Collections.Generic;
using TheSaga.SagaStates;

namespace TheSaga.Tests.Sagas.SyncAndValid
{
    public class OrderState : ISagaState
    {
        public OrderState()
        {

        }

        public Guid CorrelationID { get; set; }
        public SagaData SagaState { get; set; }
        public SagaInfo SagaInfo { get; set; }
    }
}