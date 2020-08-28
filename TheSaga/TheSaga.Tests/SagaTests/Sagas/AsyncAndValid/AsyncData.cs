using System;
using TheSaga.Models;

namespace TheSaga.Tests.SagaTests.Sagas.AsyncAndValid
{
    public class AsyncData : ISagaData
    {
        public AsyncData()
        {
        }

        public Guid ID { get; set; }
    }
}