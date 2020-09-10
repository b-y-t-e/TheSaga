using System;
using TheSaga.Models;

namespace TheSaga.Tests.SagaTests.Sagas.ResumeSaga
{
    public class ResumeSagaData : ISagaData
    {
        public ResumeSagaData()
        {

        }

        public Guid ID { get; set; }
    }
}