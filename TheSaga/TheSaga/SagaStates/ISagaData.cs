using System;
using System.Collections.Generic;

namespace TheSaga.SagaStates
{
    public interface ISagaData
    {
        public Guid CorrelationID { get; set; }
        public SagaState SagaState { get; set; }     
        public SagaInfo SagaInfo { get; set; }
    }
}