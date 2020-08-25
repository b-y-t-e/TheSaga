using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Models;
using TheSaga.SagaStates;
using TheSaga.States;

namespace TheSaga.Execution
{
    public interface ISagaExecutor
    {
        Task<ISagaState> Handle(Guid correlationID, ISagaModel model, IEvent @event);
    }
}