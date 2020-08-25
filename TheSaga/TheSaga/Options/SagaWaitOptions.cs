using System;
using System.Collections.Generic;
using System.Text;

namespace TheSaga.Options
{
    public class SagaWaitOptions
    {
        public TimeSpan Timeout { get; set; }

        public SagaWaitOptions()
        {
            Timeout = TimeSpan.FromSeconds(30);
        }
    }
}
