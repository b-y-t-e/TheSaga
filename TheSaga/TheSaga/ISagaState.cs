using System;

namespace TheSaga
{
    public interface ISagaState
    {
        public Guid CorrelationID { get; }

        public string CurrentState { get; }

        public string CurrentActivity { get; }
    }
}