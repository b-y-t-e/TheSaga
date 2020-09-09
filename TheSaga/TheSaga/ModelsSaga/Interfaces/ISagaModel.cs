using System;
using System.Collections.Generic;
using TheSaga.Models;
using TheSaga.SagaModels.Actions;
using TheSaga.SagaModels.Steps;

namespace TheSaga.SagaModels
{


    public interface ISagaModel
    {
        ISagaActions Actions { get; }
        Type SagaStateType { get; }
        string Name { get; set; }
        ESagaHistoryPolicy HistoryPolicy { get; set; }
        ESagaResumePolicy ResumePolicy { get; set; }
    }

    public enum ESagaHistoryPolicy
    {
        StoreOnlyCurrentStep = 0,
        StoreEverything = 1
    }

    public enum ESagaResumePolicy
    {
        DoCurrentStepCompensation = 0,
        DoFullCompensation = 1,
        DoNothing = 2
    }
}