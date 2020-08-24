using System;
using System.Threading.Tasks;
using TheSaga.Interfaces;
using TheSaga.Models;
using TheSaga.States;

namespace TheSaga.Executors
{
    public interface ISagaExecutor
    {
        Task<ISagaState> Handle(Guid correlationID, ISagaModel model, IEvent @event);
    }
}