using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace HuH.Communication.Transport.Tcp
{
    interface ITcpConnection
    {

        Guid ConnectionId { get; }

        bool IsConnected { get; }

        EndPoint RemoteEndPoint { get; }
        EndPoint LocalEndPoint { get; }

        void QueueMessage(byte[] message);
        void Close();

        void Close(string reason);

    }
}
