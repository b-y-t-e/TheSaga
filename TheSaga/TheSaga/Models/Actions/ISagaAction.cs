using System;
using System.Collections.Generic;
using TheSaga.Models.Steps;

namespace TheSaga.Models.Actions
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