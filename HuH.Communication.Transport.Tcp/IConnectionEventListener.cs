using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HuH.Communication.Transport.Tcp
{
    public interface IConnectionEventListener
    {
        /// <summary>
        /// 接受
        /// </summary>
        /// <param name="connection"></param>
        void OnConnectionAccepted(ITcpConnection connection);
        /// <summary>
        /// 建立
        /// </summary>
        /// <param name="connection"></param>
        void OnConnectionEstablished(ITcpConnection connection);
        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="remotingEndPoint"></param>
        /// <param name="socketError"></param>
        void OnConnectionFailed(EndPoint remotingEndPoint, SocketError socketError);

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="socketError"></param>
        void OnConnectionClosed(ITcpConnection connection, SocketError socketError);
    }
}
