using System;
using System.Collections.Generic;

namespace TheSaga.SagaStates
{
    public class SagaData
    {
        public Exception SagaCurrentError { get; set; }
        public string SagaCurrentState { get; set; }
        public string SagaCurrentStep { get; set; }
        public bool SagaIsCompensating { get; set; }

        public SagaData()
        {
        }
    }
}