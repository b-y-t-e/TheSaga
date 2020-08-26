using System;
using System.Collections.Generic;

namespace TheSaga.SagaStates
{
    public interface ISagaState
    {
        public Guid CorrelationID { get; set; }
        public Exception SagaCurrentError { get; set; }
        public string SagaCurrentState { get; set; }
        public string SagaCurrentStep { get; set; }
        public bool SagaIsCompensating { get; set; }
        public IList<SagaStepHistory> SagaHistory { get; set; }
        public DateTime SagaCreated { get; set; }
        public DateTime SagaModified { get; set; }
    }
}