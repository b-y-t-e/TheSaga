using System;
using System.Collections.Generic;
using TheSaga.SagaModels.Steps;

namespace TheSaga.SagaModels.Actions
{
    public interface ISagaAction
    {
        SagaSteps ChildSteps { get; }
        Type Event { get; }
        string State { get; }
    }
}