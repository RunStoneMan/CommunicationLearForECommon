using System;
using System.Collections.Generic;
using System.Text;

namespace HuH.Communication.Transport.Tcp.Remoting
{
    public class SocketRequestHandlerContext : IRequestHandlerContext
    {
        public ITcpConnection Connection { get; private set; }
        public Action<TcpPackage> SendRemotingResponse { get; private set; }

        public SocketRequestHandlerContext(ITcpConnection connection, Action<byte[]> sendReplyAction)
        {
            Connection = connection;
            SendRemotingResponse = remotingResponse =>
            {
                sendReplyAction(remotingResponse.AsArraySegment().Array);
            };
        }
    }
}
