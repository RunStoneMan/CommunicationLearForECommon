using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HuH.Communication.Transport.Tcp.Remoting
{
    public class ResponseFuture
    {
        private TaskCompletionSource<TcpPackage> _taskSource;

        public DateTime BeginTime { get; private set; }
        public long TimeoutMillis { get; private set; }
        public TcpPackage Request { get; private set; }

        public ResponseFuture(TcpPackage request, long timeoutMillis, TaskCompletionSource<TcpPackage> taskSource)
        {
            Request = request;
            TimeoutMillis = timeoutMillis;
            _taskSource = taskSource;
            BeginTime = DateTime.Now;
        }

        public bool IsTimeout()
        {
            return (DateTime.Now - BeginTime).TotalMilliseconds > TimeoutMillis;
        }
        public bool SetResponse(TcpPackage response)
        {
            return _taskSource.TrySetResult(response);
        }
    }
}
