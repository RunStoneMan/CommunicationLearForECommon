using System;
using System.Collections.Generic;
using System.Text;

namespace HuH.Communication.Transport.Tcp
{
    public static class TcpConfiguration
    {
        public const int SendBufferSize = 1024 * 64;
        public const int ReceiveBufferSize = 1024 * 64;
        
        public const int MaxSendPacketSize = 1024 * 64;
        public const int SendMessageFlowControlThreshold = 1000;

        public const int SocketCloseTimeoutMs = 500;

        public const int AcceptBacklogCount = 1000;
        public const int ConcurrentAccepts = 1;
        public const int AcceptPoolSize = ConcurrentAccepts * 2;

        public const int ConnectPoolSize = 32;
        public const int SendReceivePoolSize = 512;

        public const int BufferChunksCount = 512;
        public const int SocketBufferSize = 8 * 1024;

        public const int ReceiveDataBufferSize = 1024 * 64;
        public const int ReceiveDataBufferPoolSize = 50;


        public const int ReconnectToServerInterval = 5000;
        public const int ScanTimeoutRequestInterval = 5000;
    }
}
