using System;

namespace BaseProject.Api.Exceptions
{
    public class BaseProjectResponseException : Exception
    {
        public BaseProjectResponseException(string message) : base(message)
        {
        }
    }
}
