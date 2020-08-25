using System;
using TheSaga.Exceptions;

namespace TheSaga.Utils
{
    internal static class SagaExceptionExtensions
    {
        internal static Exception ToSagaStepException(this Exception exception)
        {
            if (exception == null)
                return null;

            Type exceptionType = exception.GetType();
            if (exceptionType.IsSerializable)
            {
                return exception;
            }
            else
            {
                return new SagaStepException(
                    exception.Message,
                    exception.StackTrace);
            }
        }
    }
}