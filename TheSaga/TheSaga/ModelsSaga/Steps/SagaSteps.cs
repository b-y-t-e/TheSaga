using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheSaga.ModelsSaga.Steps.Interfaces;

namespace TheSaga.ModelsSaga.Steps
{
    public class SagaSteps  : IEnumerable<ISagaStep>
    {
        List<ISagaStep> list = new List<ISagaStep>();

        public SagaSteps()
        {
            list = new List<ISagaStep>();
        }

        public SagaSteps(IEnumerable<ISagaStep> steps)
        {
            list.AddRange(steps);
        }

        public SagaSteps(params ISagaStep[] steps)
        {
            list.AddRange(steps);
        }

        public void AddStep(ISagaStep step)
        {
            list.Add(step);
        }

        public void RemoveEmptyStepsAtBeginning()
        {
            while (list.Count > 0 && list[0] is SagaEmptyStep)
                list.RemoveAt(0);
        }

        public ISagaStep GetFirstStep()
        {
            return this.FirstOrDefault();
        }

        public bool Any()
        {
            return list.Count > 0;
        }

        public IEnumerator<ISagaStep> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }
    }
}
