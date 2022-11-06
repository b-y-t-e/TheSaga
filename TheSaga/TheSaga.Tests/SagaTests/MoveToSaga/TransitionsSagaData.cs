using System;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.Tests.SagaTests.MoveToSaga
{
    public class TransitionsSagaData : ISagaData
    {
        public TransitionsSagaData()
        {

        }

        public Guid ID { get; set; }
    }
}
