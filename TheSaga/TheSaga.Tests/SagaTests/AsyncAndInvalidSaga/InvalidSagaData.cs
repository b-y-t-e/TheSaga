using System;
using TheSaga.Models;

namespace TheSaga.Tests.SagaTests.Sagas.AsyncAndInvalidSaga
{
    public class InvalidSagaData : ISagaData
    {
        public InvalidSagaData()
        {

        }

        public Guid ID { get; set; }
    }
}