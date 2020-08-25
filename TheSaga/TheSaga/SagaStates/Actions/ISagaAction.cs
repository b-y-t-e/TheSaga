using System;
using System.Collections.Generic;
using TheSaga.SagaStates.Steps;

namespace TheSaga.SagaStates.Actions
{
    public interface ISagaAction
    {
        String State { get; }

        Type Event { get; }

        List<ISagaStep> Steps { get; }

        ISagaStep FindStep(string stepName);

        ISagaStep FindNextAfter(ISagaStep step);
    }
}