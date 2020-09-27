using System;
using System.Collections.Generic;

namespace TheSaga.Models.History.Retry
{
    public class StepRetryDelayTime
    {
        public List<TimeSpan> Delays { get; set; }

        public StepRetryDelayTime()
        {
            Delays = new List<TimeSpan>();
        }
    }
}
