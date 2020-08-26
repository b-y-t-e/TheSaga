using System;
using System.Collections.Generic;

namespace TheSaga.SagaStates
{
    public interface ISagaState
    {
        public Guid CorrelationID { get; set; }
        public SagaData SagaState { get; set; }     
        public SagaInfo SagaInfo { get; set; }
    }
}