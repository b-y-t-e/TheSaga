using System;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.Tests.SagaTests.AsyncLockingSaga
{
    public class AsyncData : ISagaData
    {
        public AsyncData()
        {

        }

        public Guid ID { get; set; }
    }
}
