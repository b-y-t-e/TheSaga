using System;
using TheSaga.Models;

namespace TheSaga.Tests.SagaTests.Sagas.TransitionsSaga
{
    public class TransitionsSagaData : ISagaData
    {
        public TransitionsSagaData()
        {

        }

        public Guid ID { get; set; }
    }
}