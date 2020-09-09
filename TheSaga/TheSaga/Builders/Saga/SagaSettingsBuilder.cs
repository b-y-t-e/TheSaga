using TheSaga.Builders.Saga;
using TheSaga.SagaModels;

namespace TheSaga.Builders
{
    public class SagaSettingsBuilder : ISagaSettingsBuilder
    {
        private readonly ISagaModel model;

        public SagaSettingsBuilder(ISagaModel model)
        {
            this.model = model;
        }

        public ISagaSettingsBuilder InHistoryStoreEverything()
        {
            model.HistoryPolicy = ESagaHistoryPolicy.StoreEverything;
            return this;
        }

        public ISagaSettingsBuilder InHistoryStoreOnlyCurrentStep()
        {
            model.HistoryPolicy = ESagaHistoryPolicy.StoreOnlyCurrentStep;
            return this;
        }

        public ISagaSettingsBuilder OnResumeDoCurrentStepCompensation()
        {
            model.ResumePolicy = ESagaResumePolicy.DoCurrentStepCompensation;
            return this;
        }

        public ISagaSettingsBuilder OnResumeDoFullCompensation()
        {
            model.ResumePolicy = ESagaResumePolicy.DoFullCompensation;
            return this;
        }

        public ISagaSettingsBuilder OnResumeDoNothing()
        {
            model.ResumePolicy = ESagaResumePolicy.DoNothing;
            return this;
        }
    }
}