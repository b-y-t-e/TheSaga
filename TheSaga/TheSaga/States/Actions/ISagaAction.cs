using System;
using System.Collections.Generic;

namespace TheSaga.States.Actions
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