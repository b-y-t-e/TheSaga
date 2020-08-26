using System;
using System.Collections.Generic;

namespace TheSaga.SagaStates
{
    public class SagaInfo
    {
        public IList<SagaStepHistory> SagaHistory { get; set; }
        public DateTime SagaCreated { get; set; }
        public DateTime SagaModified { get; set; }
        public SagaInfo()
        {
            SagaHistory = new List<SagaStepHistory>();
        }
    }
}