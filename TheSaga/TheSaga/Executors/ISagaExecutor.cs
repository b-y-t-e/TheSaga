using System;
using System.Threading.Tasks;
using TheSaga.Interfaces;
using TheSaga.Models;
using TheSaga.States;

namespace TheSaga.Executors
{
    public interface ISagaExecutor
    {
        Task<ISagaState> Handle(ISagaModel model, IEvent @event);
        Task<ISagaState> Start(ISagaModel model, IEvent @event);
    }
}