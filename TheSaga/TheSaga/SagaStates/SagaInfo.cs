using System;
using System.Collections.Generic;

namespace TheSaga.SagaStates
{
    public class SagaInfo
    {
        public IList<SagaStepHistory> History { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public SagaInfo()
        {
            History = new List<SagaStepHistory>();
        }
    }
}