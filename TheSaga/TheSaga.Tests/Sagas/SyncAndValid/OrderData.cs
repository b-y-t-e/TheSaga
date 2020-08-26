using System;
using System.Collections.Generic;
using TheSaga.SagaStates;

namespace TheSaga.Tests.Sagas.SyncAndValid
{
    public class OrderData : ISagaData
    {
        public OrderData()
        {

        }

        public Guid CorrelationID { get; set; }
        public SagaState SagaState { get; set; }
        public SagaInfo SagaInfo { get; set; }
    }
}