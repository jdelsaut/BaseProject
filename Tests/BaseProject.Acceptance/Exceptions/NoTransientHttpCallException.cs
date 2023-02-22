using System;
using System.Collections.Generic;
using System.Text;

namespace BaseProject.Acceptance.Exceptions
{
    public class NoTransientHttpCallException : Exception
    {
        public NoTransientHttpCallException(string message) : base(message)
        {

        }
    }
}
