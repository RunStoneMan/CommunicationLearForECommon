using HuH.Communication.Common.Messages;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace HuH.Communication.Transport.Tcp.TcpMessages
{
    public static class TcpMessage
    {
        public class Heartbeat : Message
        {
            private static readonly int TypeId = System.Threading.Interlocked.Increment(ref NextMsgId);
            public override int MsgTypeId { get { return TypeId; } }

            public readonly int MessageNumber;

            public Heartbeat(int messageNumber)
            {
                MessageNumber = messageNumber;
            }
        }

        public class HeartbeatTimeout : Message
        {
            private static readonly int TypeId = System.Threading.Interlocked.Increment(ref NextMsgId);
            public override int MsgTypeId { get { return TypeId; } }

            public readonly int MessageNumber;

            public HeartbeatTimeout(int messageNumber)
            {
                MessageNumber = messageNumber;
            }
        }

    }
}
