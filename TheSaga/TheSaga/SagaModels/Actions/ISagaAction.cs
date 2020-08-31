using System;
using System.Collections.Generic;
using TheSaga.SagaModels.Steps;

namespace TheSaga.SagaModels.Actions
{
    public interface ISagaAction
    {
        Type Event { get; }
        string State { get; }
        List<ISagaStep> Steps { get; }

        ISagaStep FindNextAfter(ISagaStep step);

        ISagaStep FindPrevBefore(ISagaStep step);

        ISagaStep FindStep(string stepName);
    }
}