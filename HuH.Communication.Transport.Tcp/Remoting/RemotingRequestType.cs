using System;
using System.Collections.Generic;
using System.Text;

namespace HuH.Communication.Transport.Tcp.Remoting
{
    public class RemotingRequestType
    {
        public const short Async = 1;
        public const short Oneway = 2;
        public const short Callback = 3;
    }
}
