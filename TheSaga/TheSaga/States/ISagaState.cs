using System;

namespace TheSaga.States
{
    public interface ISagaState
    {
        public Guid CorrelationID { get; }

        public string CurrentState { get; set; }

        public string CurrentActivity { get; set; }
    }
}