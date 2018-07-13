using System;
using System.Collections.Generic;
using System.Text;

namespace HuH.Communication.BufferManagement.BufferManager
{
    public class UnableToCreateMemoryException : Exception
    {
        public UnableToCreateMemoryException()
            : base("All buffers were in use and acquiring more memory has been disabled.")
        {
        }
    }
}
