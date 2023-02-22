using System;

namespace BaseProject.Api.Exceptions
{
    public class BaseProjectRequestException : Exception
    {
        public BaseProjectRequestException(string message) : base(message)
        {
        }
    }
}
