using System;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.Tests.SagaTests.SendTests
{
    public class SendTestsData : ISagaData
    {
        public SendTestsData()
        {

        }

        public Guid ID { get; set; }

        public Guid CreatedSagaID { get; set; }
    }
}
