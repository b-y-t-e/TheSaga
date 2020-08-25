using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Models;
using TheSaga.SagaStates;

namespace TheSaga.Execution
{
    public interface ISagaExecutor
    {
        Task<ISagaState> Handle(Guid correlationID, ISagaModel model, IEvent @event);
    }
}