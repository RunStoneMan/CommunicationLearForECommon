using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace HuH.Communication.Transport.Tcp.Remoting.Exceptions
{
    public class RemotingTimeoutException : Exception
    {
        public RemotingTimeoutException(EndPoint serverEndPoint, TcpPackage request, long timeoutMillis)
            : base(string.Format("Wait response from server[{0}] timeout, request:{1}, timeoutMillis:{2}ms", serverEndPoint, request, timeoutMillis))
        {
        }
    }
}
