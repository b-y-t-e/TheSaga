using System;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.Tests.SagaTests.AsyncAndInvalidSaga
{
    public class InvalidSagaData : ISagaData
    {
        public InvalidSagaData()
        {

        }

        public Guid ID { get; set; }
    }
}
