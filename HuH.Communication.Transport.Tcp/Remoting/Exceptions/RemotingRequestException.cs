using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace HuH.Communication.Transport.Tcp.Remoting.Exceptions
{
    public class RemotingRequestException : Exception
    {
        public RemotingRequestException(EndPoint serverEndPoint, TcpPackage request, string errorMessage)
            : base(string.Format("Send request {0} to server [{1}] failed, errorMessage:{2}", request, serverEndPoint, errorMessage))
        {
        }
        public RemotingRequestException(EndPoint serverEndPoint, TcpPackage request, Exception exception)
            : base(string.Format("Send request {0} to server [{1}] failed.", request, serverEndPoint), exception)
        {
        }
    }
}
