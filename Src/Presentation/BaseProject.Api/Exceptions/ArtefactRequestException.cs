using System;

namespace BoxApi.Api.Exceptions
{
    public class BoxApiRequestException : Exception
    {
        public BoxApiRequestException(string message) : base(message)
        {
        }
    }
}
