﻿using System;
using TheSaga.Models;

namespace TheSaga.Tests.SagaTests.Sagas.SendTests
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