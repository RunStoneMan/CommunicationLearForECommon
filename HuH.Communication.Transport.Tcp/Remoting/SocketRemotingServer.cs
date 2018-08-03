using HuH.Communication.BufferManagement.BufferPool.ByteBuffer;
using HuH.Communication.Common.Components;
using HuH.Communication.Common.Logger;
using HuH.Communication.Common.Utils;
using HuH.Communication.Transport.Tcp.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace HuH.Communication.Transport.Tcp.Remoting
{
    public class SocketRemotingServer
    {
        private readonly TcpServerListener _serverSocket;
        private readonly Dictionary<byte, ITcpPackageRequesHandler> _requestHandlerDict;
        private readonly IByteBufferPool _receiveDataBufferPool;
        private readonly ILogger _logger;

        private bool _isShuttingdown = false;

        public IByteBufferPool BufferPool
        {
            get { return _receiveDataBufferPool; }
        }
        public TcpServerListener ServerSocket
        {
            get { return _serverSocket; }
        }

        public SocketRemotingServer() : this("Server", new IPEndPoint(SocketUtils.GetLocalIPV4(), 5000)) { }
        public SocketRemotingServer(string name, IPEndPoint listeningEndPoint)
        {
            _receiveDataBufferPool = new ByteBuffer(TcpConfiguration.ReceiveDataBufferSize, TcpConfiguration.ReceiveDataBufferPoolSize);
            _serverSocket = new TcpServerListener(listeningEndPoint, _receiveDataBufferPool, HandleRemotingRequest);
            _requestHandlerDict = new Dictionary<byte, ITcpPackageRequesHandler>();
            _logger = ObjectContainer.Resolve<ILoggerFactory>().Create(name ?? GetType().Name);
        }

        public SocketRemotingServer RegisterConnectionEventListener(IConnectionEventListener listener)
        {
            _serverSocket.RegisterConnectionEventListener(listener);
            return this;
        }
        public SocketRemotingServer Start()
        {
            _isShuttingdown = false;
            _serverSocket.Start();
            return this;
        }
        public SocketRemotingServer Shutdown()
        {
            _isShuttingdown = true;
            _serverSocket.Shutdown();
            return this;
        }
        public SocketRemotingServer RegisterRequestHandler(byte requestCode, ITcpPackageRequesHandler requestHandler)
        {
            _requestHandlerDict[requestCode] = requestHandler;
            return this;
        }
        public void PushMessageToAllConnections(TcpPackage message)
        {
            _serverSocket.PushMessageToAllConnections(message.AsArraySegment().Array);
        }
        public void PushMessageToConnection(Guid connectionId, byte[] message)
        {
            _serverSocket.PushMessageToConnection(connectionId, message);
        }
        public IList<ITcpConnection> GetAllConnections()
        {
            return _serverSocket.GetAllConnections();
        }

        private void HandleRemotingRequest(ITcpConnection connection, byte[] message, Action<byte[]> sendReplyAction)
        {
            if (_isShuttingdown) return;

            var remotingRequest = TcpPackage.FromArraySegment(message);
            var requestHandlerContext = new SocketRequestHandlerContext(connection, sendReplyAction);

            ITcpPackageRequesHandler requestHandler;
            if (!_requestHandlerDict.TryGetValue((byte)remotingRequest.Command, out requestHandler))
            {
                var errorMessage = string.Format("No request handler found for remoting request:{0}", remotingRequest);
                _logger.Error(errorMessage);
                if (remotingRequest.Command != TcpCommand.NoAnswer)
                {
                    requestHandlerContext.SendRemotingResponse(new TcpPackage(
                      TcpCommand.CommonResponseCommand,
                        remotingRequest.Sequence,
                         OperationResult.Error,
                         Encoding.UTF8.GetBytes(errorMessage),
                        remotingRequest.Header));
                }
                return;
            }

            try
            {
                var remotingResponse = requestHandler.HandleRequest(remotingRequest);
                   requestHandlerContext.SendRemotingResponse(remotingResponse);
               
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format("Unknown exception raised when handling remoting request:{0}.", remotingRequest);
                _logger.Error(errorMessage, ex);
                if (remotingRequest.Command != TcpCommand.NoAnswer)
                {
                    requestHandlerContext.SendRemotingResponse(new TcpPackage(
                     TcpCommand.CommonResponseCommand,
                       remotingRequest.Sequence,
                        OperationResult.Error,
                        Encoding.UTF8.GetBytes(errorMessage),
                       remotingRequest.Header));
                }
            }
        }
    }
}
