using System;
using System.Collections.Generic;

namespace TheSaga.SagaStates
{
    public class SagaState
    {
        public Exception CurrentError { get; set; }
        public string CurrentState { get; set; }
        public string CurrentStep { get; set; }
        public bool IsCompensating { get; set; }

        public SagaState()
        {
        }
    }
}