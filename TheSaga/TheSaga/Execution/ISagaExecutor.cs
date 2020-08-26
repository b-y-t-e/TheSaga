using System;
using System.ComponentModel;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Models;
using TheSaga.SagaStates;

namespace TheSaga.Execution
{
    internal interface ISagaExecutor
    {
        Task<ISagaState> Handle(Guid correlationID, IEvent @event, Boolean async);
    }
}