using System;
using TheSaga.ModelsSaga.Actions.Interfaces;

namespace TheSaga.ModelsSaga.Interfaces
{
    public interface ISagaModel
    {
        ISagaActions Actions { get; }
        Type SagaStateType { get; }
        string Name { get; set; }
        ESagaResumePolicy ResumePolicy { get; set; }
    }
}
