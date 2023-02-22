using System;

namespace BoxApi.Api.Exceptions
{
    public class BoxApiResponseException : Exception
    {
        public BoxApiResponseException(string message) : base(message)
        {
        }
    }
}
