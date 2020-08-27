using System;
using TheSaga.SagaStates;

namespace TheSaga.SagaStates
{
    public interface ISaga : ISagaState
    {
        ISagaData Data { get; }
    }

    public interface ISagaState
    {
        SagaInfo Info { get; }

        SagaState State { get; }
    }
}