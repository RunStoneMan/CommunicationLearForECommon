using HuH.Communication.BufferManagement.BufferPool.ByteBuffer;
using HuH.Communication.Common.Components;
using HuH.Communication.Common.Logger;
using HuH.Communication.Common.Scheduling;
using HuH.Communication.Common.Utils;
using HuH.Communication.Transport.Tcp.Remoting.Exceptions;
using HuH.Communication.Transport.Tcp.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HuH.Communication.Transport.Tcp.Remoting
{
    public class SocketRemotingClient
    {
        private readonly byte[] TimeoutMessage = Encoding.UTF8.GetBytes("Remoting request timeout.");
        private readonly Dictionary<byte, ITcpPackageHandler> _responseHandlerDict;


        private readonly IList<IConnectionEventListener> _connectionEventListeners;
        private readonly ConcurrentDictionary<long, ResponseFuture> _responseFutureDict;

        private readonly BlockingCollection<byte[]> _replyMessageQueue;
        private readonly IScheduleService _scheduleService;
        private readonly IByteBufferPool _receiveDataBufferPool;
        private readonly ILogger _logger;


        private EndPoint _serverEndPoint;
        private EndPoint _localEndPoint;
        private TcpClientConnector _clientSocket;
        private int _reconnecting = 0;
        private bool _shutteddown = false;
        private bool _started = false;

        public bool IsConnected
        {
            get { return _clientSocket != null && _clientSocket.IsConnected; }
        }
        public EndPoint LocalEndPoint
        {
            get { return _localEndPoint; }
        }
        public EndPoint ServerEndPoint
        {
            get { return _serverEndPoint; }
        }
        public TcpClientConnector ClientSocket
        {
            get { return _clientSocket; }
        }
        public IByteBufferPool BufferPool
        {
            get { return _receiveDataBufferPool; }
        }

        public SocketRemotingClient() : this(new IPEndPoint(SocketUtils.GetLocalIPV4(), 5000)) { }
        public SocketRemotingClient(EndPoint serverEndPoint, EndPoint localEndPoint = null)
        {
            Ensure.NotNull(serverEndPoint, "serverEndPoint");

            _serverEndPoint = serverEndPoint;
            _localEndPoint = localEndPoint;
            _receiveDataBufferPool = new ByteBuffer(TcpConfiguration.ReceiveDataBufferSize, TcpConfiguration.ReceiveDataBufferPoolSize);
            _clientSocket = new TcpClientConnector(_serverEndPoint, _localEndPoint, _receiveDataBufferPool, HandleServerMessage);
            _responseFutureDict = new ConcurrentDictionary<long, ResponseFuture>();
            _replyMessageQueue = new BlockingCollection<byte[]>(new ConcurrentQueue<byte[]>());
            _responseHandlerDict = new Dictionary<byte, ITcpPackageHandler>();

            _connectionEventListeners = new List<IConnectionEventListener>();
            _scheduleService = ObjectContainer.Resolve<IScheduleService>();
            _logger = ObjectContainer.Resolve<ILoggerFactory>().Create(GetType().FullName);

            RegisterConnectionEventListener(new ConnectionEventListener(this));
        }

        public SocketRemotingClient RegisterResponseHandler(byte requestCode, ITcpPackageHandler responseHandler)
        {
            _responseHandlerDict[requestCode] = responseHandler;
            return this;
        }

        public SocketRemotingClient RegisterConnectionEventListener(IConnectionEventListener listener)
        {
            _connectionEventListeners.Add(listener);
            _clientSocket.RegisterConnectionEventListener(listener);
            return this;
        }
        public SocketRemotingClient Start()
        {
            if (_started) return this;

            StartClientSocket();
            StartScanTimeoutRequestTask();
            _shutteddown = false;
            _started = true;
            return this;
        }
        public void Shutdown()
        {
            _shutteddown = true;
            StopReconnectServerTask();
            StopScanTimeoutRequestTask();
            ShutdownClientSocket();
        }

        public Task<TcpPackage> InvokeAsync(TcpPackage request, int timeoutMillis)
        {
            EnsureClientStatus();
            var taskCompletionSource = new TaskCompletionSource<TcpPackage>();
            var responseFuture = new ResponseFuture(request, timeoutMillis, taskCompletionSource);

            if (!_responseFutureDict.TryAdd(request.Sequence, responseFuture))
            {
                throw new ResponseFutureAddFailedException(request.Sequence);
            }

            _clientSocket.QueueMessage(request.AsArraySegment().Array);

            return taskCompletionSource.Task;
        }

        public void InvokeOneway(TcpPackage request)
        {
            EnsureClientStatus();
            _clientSocket.QueueMessage(request.AsArraySegment().Array);
        }

        private void HandleServerMessage(ITcpConnection connection, byte[] message)
        {
            if (message == null) return;

            var remotingServerMessage = TcpPackage.FromArraySegment(message);

            HandleResponseMessage(connection, remotingServerMessage);
        }
        private void HandleResponseMessage(ITcpConnection connection, TcpPackage message)
        {
            if (message == null) return;

            if (message.Command == TcpCommand.CommonResponseCommand)
            {
                ResponseFuture responseFuture;
                if (_responseFutureDict.TryRemove(message.Sequence, out responseFuture))
                {
                    if (responseFuture.SetResponse(message))
                    {
                        if (_logger.IsDebugEnabled)
                        {
                            _logger.DebugFormat("Remoting response back, request code:{0}, requect sequence:{1}, time spent:{2}", message.Command, responseFuture.Request.Sequence, (DateTime.Now - responseFuture.BeginTime).TotalMilliseconds);
                        }
                    }
                    else
                    {
                        _logger.ErrorFormat("Set remoting response failed, response:" + message.ToString());
                    }
                }
                else
                {
                    ITcpPackageHandler responseHandler;
                    if (_responseHandlerDict.TryGetValue((byte)message.Command, out responseHandler))
                    {
                        responseHandler.HandleResponse(message);
                    }
                    else
                    {
                        _logger.ErrorFormat("No response handler found for remoting response:{0}", message.ToString());

                    }
                }
                if (message.Command == TcpCommand.NotHandled)
                {
                    string requestId;
                    if (message.Header.TryGetValue("requestId", out requestId))
                    {
                        _logger.InfoFormat("Remoting response back ,request Message Id {0}", requestId);
                    }

                }
            }
            else
            {
                ITcpPackageHandler responseHandler;
                if (_responseHandlerDict.TryGetValue((byte)message.Command, out responseHandler))
                {
                    responseHandler.HandleResponse(message);
                }
                else
                {
                    _logger.ErrorFormat("No response handler found for remoting response:{0}", message.ToString());

                }
            }
        }
        private void ScanTimeoutRequest()
        {
            var timeoutKeyList = new List<long>();
            foreach (var entry in _responseFutureDict)
            {
                if (entry.Value.IsTimeout())
                {
                    timeoutKeyList.Add(entry.Key);
                }
            }
            foreach (var key in timeoutKeyList)
            {
                ResponseFuture responseFuture;
                if (_responseFutureDict.TryRemove(key, out responseFuture))
                {
                    var request = responseFuture.Request;
                    responseFuture.SetResponse(new TcpPackage(
                        request.Command,
                        request.Sequence,
                        null,
                        request.Header
                        ));
                    if (_logger.IsDebugEnabled)
                    {
                        _logger.DebugFormat("Removed timeout request:{0}", responseFuture.Request);
                    }
                }
            }
        }
        private void ReconnectServer()
        {
            _logger.InfoFormat("Try to reconnect to server, address: {0}", _serverEndPoint);

            if (_clientSocket.IsConnected) return;
            if (!EnterReconnecting()) return;

            try
            {
                _clientSocket.Shutdown();
                _clientSocket = new TcpClientConnector(_serverEndPoint, _localEndPoint, _receiveDataBufferPool, HandleServerMessage);
                foreach (var listener in _connectionEventListeners)
                {
                    _clientSocket.RegisterConnectionEventListener(listener);
                }
                _clientSocket.Start();
            }
            catch (Exception ex)
            {
                _logger.Error("Reconnect to server error.", ex);
                ExitReconnecting();
            }
        }
        private void StartClientSocket()
        {
            _clientSocket.Start();
        }
        private void ShutdownClientSocket()
        {
            _clientSocket.Shutdown();
        }
        private void StartScanTimeoutRequestTask()
        {
            _scheduleService.StartTask(string.Format("{0}.ScanTimeoutRequest", this.GetType().Name), ScanTimeoutRequest, 1000, TcpConfiguration.ScanTimeoutRequestInterval);
        }
        private void StopScanTimeoutRequestTask()
        {
            _scheduleService.StopTask(string.Format("{0}.ScanTimeoutRequest", this.GetType().Name));
        }
        private void StartReconnectServerTask()
        {
            _scheduleService.StartTask(string.Format("{0}.ReconnectServer", this.GetType().Name), ReconnectServer, 1000, TcpConfiguration.ReconnectToServerInterval);
        }
        private void StopReconnectServerTask()
        {
            _scheduleService.StopTask(string.Format("{0}.ReconnectServer", this.GetType().Name));
        }
        private void EnsureClientStatus()
        {
            if (_clientSocket == null || !_clientSocket.IsConnected)
            {
                throw new RemotingServerUnAvailableException(_serverEndPoint);
            }
        }
        private bool EnterReconnecting()
        {
            return Interlocked.CompareExchange(ref _reconnecting, 1, 0) == 0;
        }
        private void ExitReconnecting()
        {
            Interlocked.Exchange(ref _reconnecting, 0);
        }
        private void SetLocalEndPoint(EndPoint localEndPoint)
        {
            _localEndPoint = localEndPoint;
        }

        class ConnectionEventListener : IConnectionEventListener
        {
            private readonly SocketRemotingClient _remotingClient;

            public ConnectionEventListener(SocketRemotingClient remotingClient)
            {
                _remotingClient = remotingClient;
            }

            public void OnConnectionAccepted(ITcpConnection connection) { }
            public void OnConnectionEstablished(ITcpConnection connection)
            {
                _remotingClient.StopReconnectServerTask();
                _remotingClient.ExitReconnecting();
                _remotingClient.SetLocalEndPoint(connection.LocalEndPoint);
            }
            public void OnConnectionFailed(EndPoint remotingEndPoint, SocketError socketError)
            {
                if (_remotingClient._shutteddown) return;

                _remotingClient.ExitReconnecting();
                _remotingClient.StartReconnectServerTask();
            }
            public void OnConnectionClosed(ITcpConnection connection, SocketError socketError)
            {
                if (_remotingClient._shutteddown) return;

                _remotingClient.ExitReconnecting();
                _remotingClient.StartReconnectServerTask();
            }
        }
    }
}
