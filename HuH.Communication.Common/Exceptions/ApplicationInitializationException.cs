using System;
using System.Collections.Generic;
using System.Text;

namespace HuH.Communication.Common.Exceptions
{
    public class ApplicationInitializationException : Exception
    {
        public ApplicationInitializationException(string message) : base(message) { }
        public ApplicationInitializationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
