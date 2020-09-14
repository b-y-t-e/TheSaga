using System;
using TheSaga.Events;
using TheSaga.ModelsSaga.History;
using TheSaga.ValueObjects;

namespace TheSaga.Models
{
    public interface ISagaExecutionState
    {
        AsyncExecution AsyncExecution { get; }
        Exception CurrentError { get; }
        ISagaEvent CurrentEvent { get; }
        string CurrentState { get; }
        string CurrentStep { get; }
        ExecutionID ExecutionID { get; }
        SagaHistory History { get; }
        bool IsCompensating { get; }
        bool IsResuming { get; }
    }
}