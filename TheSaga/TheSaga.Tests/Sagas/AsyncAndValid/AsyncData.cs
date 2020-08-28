using System;
using System.Collections.Generic;
using TheSaga.Models;

namespace TheSaga.Tests.Sagas.AsyncAndValid
{
    public class AsyncData : ISagaData
    {
        public AsyncData()
        {
        }

        public Guid ID { get; set; }
    }
}