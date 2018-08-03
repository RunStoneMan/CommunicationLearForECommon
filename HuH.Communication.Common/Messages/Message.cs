using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HuH.Communication.Common.Messages
{
    public abstract class Message
    {
        protected static int NextMsgId = -1;
        private static readonly int TypeId = Interlocked.Increment(ref NextMsgId);
        public virtual int MsgTypeId { get { return TypeId; } }
    }
}
