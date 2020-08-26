using System;
using System.Collections.Generic;
using TheSaga.SagaStates;

namespace TheSaga.Tests.Sagas.SyncAndValid
{
    public class OrderState : ISagaState
    {
        public OrderState()
        {
            SagaHistory = new List<SagaStepHistory>();
        }

        public Guid CorrelationID { get; set; }
        public Exception SagaCurrentError { get; set; }
        public string SagaCurrentState { get; set; }
        public string SagaCurrentStep { get; set; }
        public IList<SagaStepHistory> SagaHistory { get; set; }
        public bool SagaIsCompensating { get; set; }
        public DateTime SagaCreated { get; set; }
        public DateTime SagaModified { get; set; }
    }
}