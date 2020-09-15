using Newtonsoft.Json;
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

            if (isSerializable(exception))
                return exception;

            return new SagaStepException(
                exception);
        }

        static bool isSerializable(Exception ex)
        {
            /*Type exceptionType = ex.GetType();
            if (exceptionType.IsSerializable)
                return true;*/

            try
            {
                var json = JsonConvert.SerializeObject(ex,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

                var obj = JsonConvert.DeserializeObject(json,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}