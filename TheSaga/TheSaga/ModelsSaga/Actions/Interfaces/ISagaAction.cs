using System;
using TheSaga.ModelsSaga.Steps;

namespace TheSaga.ModelsSaga.Actions.Interfaces
{
    public interface ISagaAction
    {
        SagaSteps ChildSteps { get; }
        Type Event { get; }
        string State { get; }
    }
}
