using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TheSaga.Coordinators;
using TheSaga.Locking;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.Persistance;
using TheSaga.Tests.SagaTests.ResumeSaga.Events;
using Xunit;
using TheSaga.Persistance.SqlServer;
using TheSaga.Persistance.SqlServer.Options;
using TheSaga.Locking.DistributedLock;
using TheSaga.Tests.SagaTests.ResumeSaga.States;
using TheSaga.Locking.DistributedLock.Options;
using TheSaga.ModelsSaga.History;
using Newtonsoft.Json;

namespace TheSaga.Tests.SerializationTests
{
    public class SerializationTests
    {
        [Fact]
        public async Task WHEN_sagaIsStopped_THEN_sagaShouldNotBeInIdleState()
        {
            // given
            var stepData = new StepData()
            {
                ExecutionData = new StepExecutionData()
                {
                    Error = new ProblematicException()
                }
            };

            var json = JsonConvert.SerializeObject(stepData,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

            // when
            var obj = JsonConvert.DeserializeObject(json,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Error = (s, e) =>
                    {

                        if (e.CurrentObject is StepExecutionData executionData &&
                            executionData != null &&
                            nameof(executionData.Error).Equals(e.ErrorContext.Member))
                        {
                            
                        }
                    }
                });

            // then
            //ISaga persistedSaga = await sagaPersistance.Get(saga.Data.ID);
            //persistedSaga.IsIdle().ShouldBeFalse();
        }

    }


    [Serializable]
    public class ProblematicException : Exception
    {
        public ProblematicException() { }
        public ProblematicException(string message) : base(message) { }
        public ProblematicException(string message, Exception inner) : base(message, inner) { }
        protected ProblematicException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
            throw new Exception("!!");
        }
    }
}
