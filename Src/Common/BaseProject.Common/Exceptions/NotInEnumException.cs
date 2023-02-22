using System;

namespace BoxApi.Common.Exceptions
{
    public class NotInEnumException : Exception
    {
        public NotInEnumException()
        {

        }

        public NotInEnumException(string message) : base(message)
        {

        }
    }
}
