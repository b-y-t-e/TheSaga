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
using TheSaga.Models.History;
using Newtonsoft.Json;

namespace TheSaga.Tests.SerializationTests
{
    public class SerializationTests
    {
        [Fact]
        public async Task WHEN_sagaIsStopped_THEN_sagaShouldNotBeInIdleState()
        {
            // given
            var json = SerializationTests.json;

            // when
            var obj = JsonConvert.DeserializeObject(json,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Error = (s, e) =>
                    {
                        if (e.CurrentObject is SagaExecutionState executionState)
                        {
                            if (e.ErrorContext.Member.Equals(nameof(executionState.History)))
                            {
                                executionState.History = new SagaHistory();
                                e.ErrorContext.Handled = true;
                            }
                        }
                    }
                });
        }

        static readonly string json = "{\"$type\":\"TheSaga.Models.Saga, TheSaga\",\"Data\":{\"$type\":\"TheSaga.Tests.SagaTests.SyncAndValid.OrderData, TheSaga.Tests\",\"ID\":\"199a42a7-ffa4-4954-ae4a-b81c57bb4eb5\"},\"ExecutionInfo\":{\"$type\":\"TheSaga.Models.SagaExecutionInfo, TheSaga\",\"Created\":\"2020-09-23T22:09:38.608064+02:00\",\"Modified\":\"2020-09-23T22:09:38.7602475+02:00\",\"ModelName\":\"OrderSagaBuilder\"},\"ExecutionState\":{\"$type\":\"TheSaga.Models.SagaExecutionState, TheSaga\",\"CurrentEvent\":{\"$type\":\"TheSaga.Tests.SagaTests.SyncAndValid.Events.OrderCompletedEvent, TheSaga.Tests\",\"ID\":\"199a42a7-ffa4-4954-ae4a-b81c57bb4eb5\"},\"CurrentError\":null,\"CurrentState\":\"StateCompleted\",\"CurrentStep\":null,\"IsCompensating\":false,\"IsResuming\":false,\"History\":{\"$type\":\"TheSaga.ModelsSaga.History.SagaHistory, TheSaga\",\"$values\":[{\"$type\":\"TheSaga.ModelsSaga.History.StepData, TheSaga\",\"ExecutionID\":\"68762d76-9252-4b06-bbc5-cd9ba71283b3\",\"StepName\":\"StateCreated | When | OrderCompletedEvent | #0\",\"StateName\":\"StateCreated\",\"ExecutionValues\":{\"$type\":\"TheSaga.Models.StepExecutionValues, TheSaga\"},\"ExecutionData\":{\"$type\":\"TheSaga.ModelsSaga.History.StepExecutionData, TheSaga\",\"StartTime\":\"2020-09-23T22:09:38.6651006+02:00\",\"EndTime\":\"2020-09-23T22:09:38.6738448+02:00\",\"SucceedTime\":\"2020-09-23T22:09:38.6738368+02:00\",\"FailTime\":null,\"Error\":null,\"EndStateName\":\"StateCreated\",\"NextStepName\":\"OrderCompletedEventStep1\",\"ConditionResult\":null,\"StepType\":\"TheSaga.ModelsSaga.Steps.SagaEmptyStep, TheSaga, Version=0.8.1.0, Culture=neutral, PublicKeyToken=null\"},\"CompensationData\":null,\"ResumeData\":null,\"AsyncExecution\":false,\"AsyncStep\":false,\"Event\":{\"$type\":\"TheSaga.Tests.SagaTests.SyncAndValid.Events.OrderCompletedEvent, TheSaga.Tests\",\"ID\":\"199a42a7-ffa4-4954-ae4a-b81c57bb4eb5\"}},{\"$type\":\"TheSaga.ModelsSaga.History.StepData, TheSaga\",\"ExecutionID\":\"68762d76-9252-4b06-bbc5-cd9ba71283b3\",\"StepName\":\"OrderCompletedEventStep1\",\"StateName\":\"StateCreated\",\"ExecutionValues\":{\"$type\":\"TheSaga.Models.StepExecutionValues, TheSaga\"},\"ExecutionData\":{\"$type\":\"TheSaga.ModelsSaga.History.StepExecutionData, TheSaga\",\"StartTime\":\"2020-09-23T22:09:38.6806171+02:00\",\"EndTime\":\"2020-09-23T22:09:38.6871758+02:00\",\"SucceedTime\":\"2020-09-23T22:09:38.6871687+02:00\",\"FailTime\":null,\"Error\":null,\"EndStateName\":\"StateCreated\",\"NextStepName\":\"email\",\"ConditionResult\":null,\"StepType\":\"TheSaga.ModelsSaga.Steps.SagaStepForThenInline`1[[TheSaga.Tests.SagaTests.SyncAndValid.OrderData, TheSaga.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], TheSaga, Version=0.8.1.0, Culture=neutral, PublicKeyToken=null\"},\"CompensationData\":null,\"ResumeData\":null,\"AsyncExecution\":false,\"AsyncStep\":false,\"Event\":{\"$type\":\"TheSaga.Tests.SagaTests.SyncAndValid.Events.OrderCompletedEvent, TheSaga.Tests\",\"ID\":\"199a42a7-ffa4-4954-ae4a-b81c57bb4eb5\"}},{\"$type\":\"TheSaga.ModelsSaga.History.StepData, TheSaga\",\"ExecutionID\":\"68762d76-9252-4b06-bbc5-cd9ba71283b3\",\"StepName\":\"email\",\"StateName\":\"StateCreated\",\"ExecutionValues\":{\"$type\":\"TheSaga.Models.StepExecutionValues, TheSaga\"},\"ExecutionData\":{\"$type\":\"TheSaga.ModelsSaga.History.StepExecutionData, TheSaga\",\"StartTime\":\"2020-09-23T22:09:38.6966341+02:00\",\"EndTime\":\"2020-09-23T22:09:38.7073808+02:00\",\"SucceedTime\":\"2020-09-23T22:09:38.7073203+02:00\",\"FailTime\":null,\"Error\":null,\"EndStateName\":\"StateCreated\",\"NextStepName\":\"SendMessageToTheManagerEventStep\",\"ConditionResult\":null,\"StepType\":\"TheSaga.ModelsSaga.Steps.SagaStepForThen`2[[TheSaga.Tests.SagaTests.SyncAndValid.OrderData, TheSaga.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[TheSaga.Tests.SagaTests.SyncAndValid.Activities.SendEmailToClientEvent, TheSaga.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], TheSaga, Version=0.8.1.0, Culture=neutral, PublicKeyToken=null\"},\"CompensationData\":null,\"ResumeData\":null,\"AsyncExecution\":false,\"AsyncStep\":false,\"Event\":{\"$type\":\"TheSaga.Tests.SagaTests.SyncAndValid.Events.OrderCompletedEvent, TheSaga.Tests\",\"ID\":\"199a42a7-ffa4-4954-ae4a-b81c57bb4eb5\"}},{\"$type\":\"TheSaga.ModelsSaga.History.StepData, TheSaga\",\"ExecutionID\":\"68762d76-9252-4b06-bbc5-cd9ba71283b3\",\"StepName\":\"SendMessageToTheManagerEventStep\",\"StateName\":\"StateCreated\",\"ExecutionValues\":{\"$type\":\"TheSaga.Models.StepExecutionValues, TheSaga\"},\"ExecutionData\":{\"$type\":\"TheSaga.ModelsSaga.History.StepExecutionData, TheSaga\",\"StartTime\":\"2020-09-23T22:09:38.7164121+02:00\",\"EndTime\":\"2020-09-23T22:09:38.7260875+02:00\",\"SucceedTime\":\"2020-09-23T22:09:38.7260683+02:00\",\"FailTime\":null,\"Error\":null,\"EndStateName\":\"StateCreated\",\"NextStepName\":\"OrderCourierEventStep\",\"ConditionResult\":null,\"StepType\":\"TheSaga.ModelsSaga.Steps.SagaStepForThen`2[[TheSaga.Tests.SagaTests.SyncAndValid.OrderData, TheSaga.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[TheSaga.Tests.SagaTests.SyncAndValid.Activities.SendMessageToTheManagerEvent, TheSaga.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], TheSaga, Version=0.8.1.0, Culture=neutral, PublicKeyToken=null\"},\"CompensationData\":null,\"ResumeData\":null,\"AsyncExecution\":false,\"AsyncStep\":false,\"Event\":{\"$type\":\"TheSaga.Tests.SagaTests.SyncAndValid.Events.OrderCompletedEvent, TheSaga.Tests\",\"ID\":\"199a42a7-ffa4-4954-ae4a-b81c57bb4eb5\"}},{\"$type\":\"TheSaga.ModelsSaga.History.StepData, TheSaga\",\"ExecutionID\":\"68762d76-9252-4b06-bbc5-cd9ba71283b3\",\"StepName\":\"OrderCourierEventStep\",\"StateName\":\"StateCreated\",\"ExecutionValues\":{\"$type\":\"TheSaga.Models.StepExecutionValues, TheSaga\"},\"ExecutionData\":{\"$type\":\"TheSaga.ModelsSaga.History.StepExecutionData, TheSaga\",\"StartTime\":\"2020-09-23T22:09:38.7328127+02:00\",\"EndTime\":\"2020-09-23T22:09:38.7415204+02:00\",\"SucceedTime\":\"2020-09-23T22:09:38.7415176+02:00\",\"FailTime\":null,\"Error\":null,\"EndStateName\":\"StateCreated\",\"NextStepName\":\"StateCreated | TransitionTo | StateCompleted | #0\",\"ConditionResult\":null,\"StepType\":\"TheSaga.ModelsSaga.Steps.SagaStepForThen`2[[TheSaga.Tests.SagaTests.SyncAndValid.OrderData, TheSaga.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[TheSaga.Tests.SagaTests.SyncAndValid.Activities.OrderCourierEvent, TheSaga.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], TheSaga, Version=0.8.1.0, Culture=neutral, PublicKeyToken=null\"},\"CompensationData\":null,\"ResumeData\":null,\"AsyncExecution\":false,\"AsyncStep\":false,\"Event\":{\"$type\":\"TheSaga.Tests.SagaTests.SyncAndValid.Events.OrderCompletedEvent, TheSaga.Tests\",\"ID\":\"199a42a7-ffa4-4954-ae4a-b81c57bb4eb5\"}},{\"$type\":\"TheSaga.ModelsSaga.History.StepData, TheSaga\",\"ExecutionID\":\"68762d76-9252-4b06-bbc5-cd9ba71283b3\",\"StepName\":\"StateCreated | TransitionTo | StateCompleted | #0\",\"StateName\":\"StateCreated\",\"ExecutionValues\":{\"$type\":\"TheSaga.Models.StepExecutionValues, TheSaga\"},\"ExecutionData\":{\"$type\":\"TheSaga.ModelsSaga.History.StepExecutionData, TheSaga\",\"StartTime\":\"2020-09-23T22:09:38.7493897+02:00\",\"EndTime\":\"2020-09-23T22:09:38.7602385+02:00\",\"SucceedTime\":\"2020-09-23T22:09:38.7602321+02:00\",\"FailTime\":null,\"Error\":null,\"EndStateName\":\"StateCompleted\",\"NextStepName\":null,\"ConditionResult\":null,\"StepType\":\"TheSaga.ModelsSaga.Steps.SagaStepForThenInline`1[[TheSaga.Tests.SagaTests.SyncAndValid.OrderData, TheSaga.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], TheSaga, Version=0.8.1.0, Culture=neutral, PublicKeyToken=null\"},\"CompensationData\":null,\"ResumeData\":null,\"AsyncExecution\":false,\"AsyncStep\":false,\"Event\":{\"$type\":\"TheSaga.Tests.SagaTests.SyncAndValid.Events.OrderCompletedEvent, TheSaga.Tests\",\"ID\":\"199a42a7-ffa4-4954-ae4a-b81c57bb4eb5\"}}]},\"ExecutionID\":{\"$type\":\"TheSaga.ValueObjects.ExecutionID, TheSaga\",\"Value\":\"68762d76-9252-4b06-bbc5-cd9ba71283b3\"},\"AsyncExecution\":{\"$type\":\"TheSaga.ValueObjects.AsyncExecution, TheSaga\",\"Value\":false},\"IsDeleted\":false},\"ExecutionValues\":{\"$type\":\"TheSaga.Models.SagaExecutionValues, TheSaga\"}}";
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
