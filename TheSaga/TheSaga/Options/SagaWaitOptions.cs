using System;

namespace TheSaga.Options
{
    public class SagaWaitOptions
    {
        public SagaWaitOptions()
        {
            Timeout = TimeSpan.FromSeconds(30);
        }

        public TimeSpan Timeout { get; set; }
    }
}