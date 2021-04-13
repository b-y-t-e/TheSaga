using System;
using TheSaga.Events;
using TheSaga.Models.Interfaces;
using TheSaga.ModelsSaga.Steps.Delegates;
using TheSaga.ModelsSaga.Steps.Interfaces;

namespace TheSaga.ModelsSaga.Steps
{
    public interface ISagaPublishActivity<TSagaData, TExecuteEvent, TCompensateEvent> : ISagaStep
        where TSagaData : ISagaData
        where TExecuteEvent : ISagaEvent, new()
        where TCompensateEvent : ISagaEvent, new()
    {
        SendActionAsyncDelegate<TSagaData, TExecuteEvent> ActionDelegate { get; set; }
        SendActionAsyncDelegate<TSagaData, TCompensateEvent> CompensateDelegate { get; set; }
    }
}