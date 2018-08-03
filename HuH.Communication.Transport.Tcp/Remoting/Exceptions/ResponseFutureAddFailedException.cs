using System;
using System.Collections.Generic;
using System.Text;

namespace HuH.Communication.Transport.Tcp.Remoting.Exceptions
{
    public class ResponseFutureAddFailedException : Exception
    {
        public ResponseFutureAddFailedException(long requestSequence)
            : base(string.Format("Add remoting request response future failed. request sequence:{0}", requestSequence))
        {
        }
    }
}
