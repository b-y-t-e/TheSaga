using System;

namespace TheSaga.Builders.Saga
{
    public interface ISagaSettingsBuilder
    {
        ISagaSettingsBuilder InHistoryStoreEverything();
        ISagaSettingsBuilder InHistoryStoreOnlyCurrentStep();
        ISagaSettingsBuilder OnResumeDoCurrentStepCompensation();
        ISagaSettingsBuilder OnResumeDoFullCompensation();
        ISagaSettingsBuilder OnResumeDoNothing();
    }
}