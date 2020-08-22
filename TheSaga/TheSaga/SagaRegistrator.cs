using System;

namespace TheSaga
{
    public class SagaRegistrator : ISagaRegistrator
    {
        public SagaRegistrator()
        {

        }

        public void Register<TState>(ISaga<TState> saga)
            where TState : ISagaState
        {
            throw new NotImplementedException();
        }
    }
}