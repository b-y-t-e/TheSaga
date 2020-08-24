using System;
using System.Collections.Generic;

namespace TheSaga.States
{
    public interface ISagaState
    {
        public Guid CorrelationID { get; set; }
        public string CurrentState { get; set; }
        public string CurrentStep { get; set; }
        public bool IsCompensating { get; set; }
    }
}