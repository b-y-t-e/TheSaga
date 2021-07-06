using System;

namespace TheSaga.Options
{
    public class SagaRunOptions
    {
        public SagaRunOptions()
        {
            CanBeResumed = true;
        }

        public Boolean CanBeResumed { get; set; }
    }
}