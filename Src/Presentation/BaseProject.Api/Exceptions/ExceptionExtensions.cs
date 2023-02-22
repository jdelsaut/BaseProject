using BaseProject.Application.Exceptions;
using System;

namespace BaseProject.Api.Exceptions
{
    public static class ExceptionExtensions
    {
        public static string GetExceptionMessage(this Exception exception)
        {
            return exception.InnerException != null ? exception.InnerException.Message : exception.Message;
        }

        public static string GetErrorMessage(this Exception exception)
        {
            return (exception.GetBaseException().GetType() == typeof(ValidationException) || exception.GetType() == typeof(ValidationException))? 
                exception.ToString() : exception.GetExceptionMessage();
        }
    }
}
