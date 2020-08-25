using System;
using System.Collections.Generic;
using TheSaga.SagaStates.Steps;

namespace TheSaga.SagaStates.Actions
{
    public interface ISagaAction
    {
        Type Event { get; }
        String State { get; }
        List<ISagaStep> Steps { get; }

        ISagaStep FindNextAfter(ISagaStep step);

        ISagaStep FindPrevBefore(ISagaStep step);

        ISagaStep FindStep(string stepName);
    }
}