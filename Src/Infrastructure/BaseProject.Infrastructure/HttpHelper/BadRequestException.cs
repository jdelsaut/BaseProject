using System;
using System.Collections.Generic;
using System.Text;

namespace BaseProject.Infrastructure.HttpHelper
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}
