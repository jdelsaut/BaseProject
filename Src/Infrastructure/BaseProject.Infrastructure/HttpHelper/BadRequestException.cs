using System;
using System.Collections.Generic;
using System.Text;

namespace BoxApi.Infrastructure.HttpHelper
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}
