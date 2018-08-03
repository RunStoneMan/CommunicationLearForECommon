using System;
using System.Collections.Generic;
using System.Text;

namespace HuH.Communication.Transport.Tcp.Remoting
{
    public class IRequestHandlerContext
    {
        ITcpConnection Connection { get; }
        Action<TcpPackage> SendRemotingResponse { get; }
    }
}
