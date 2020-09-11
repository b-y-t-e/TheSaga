using System;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.Tests.SagaTests.ResumeSaga
{
    public class ResumeSagaData : ISagaData
    {
        public ResumeSagaData()
        {

        }

        public Guid ID { get; set; }
    }
}
