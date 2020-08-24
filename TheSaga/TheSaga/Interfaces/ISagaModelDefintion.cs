using System;
using TheSaga.Models;
using TheSaga.States;

namespace TheSaga.Interfaces
{
    public interface ISagaModelDefintion<TSagaState> where TSagaState : ISagaState
    {
        SagaModel<TSagaState> GetModel(IServiceProvider serviceProvider);
    }
}