using System;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.Tests.SagaTests.TransitionsSaga
{
    public class TransitionsSagaData : ISagaData
    {
        public TransitionsSagaData()
        {

        }

        public Guid ID { get; set; }
    }
}
