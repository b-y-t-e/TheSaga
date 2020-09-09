using System;
using System.Runtime.Serialization;

namespace TheSaga.Exceptions
{
    [Serializable]
    public class SagaCompensateAllOnResumeException : Exception
    {
        public SagaCompensateAllOnResumeException() :
            base("Compensating all steps on resume")
        {
        }
    }
}