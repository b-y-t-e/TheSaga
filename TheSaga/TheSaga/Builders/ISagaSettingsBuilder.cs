namespace TheSaga.Builders
{
    public interface ISagaSettingsBuilder
    {
        ISagaSettingsBuilder OnResumeDoCurrentStepCompensation();
        ISagaSettingsBuilder OnResumeDoFullCompensation();
        ISagaSettingsBuilder OnResumeDoNothing();
    }
}
