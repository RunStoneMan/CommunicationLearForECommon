using System;
using System.Collections.Generic;
using System.Text;

namespace HuH.Communication.Transport.Tcp.Remoting
{
    public interface ITcpPackageHandler
    {
        void HandleResponse(TcpPackage remotingResponse);
    }

    public interface ITcpPackageRequesHandler
    {
        TcpPackage HandleRequest(TcpPackage remotingRequest);
    }
}
