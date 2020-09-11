namespace TheSaga.Builders
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
