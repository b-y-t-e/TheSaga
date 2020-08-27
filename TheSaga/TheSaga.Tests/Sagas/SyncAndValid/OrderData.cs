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

        public Guid ID { get; set; }
        public SagaState SagaState { get; set; }
    }
}