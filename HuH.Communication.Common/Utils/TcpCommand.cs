using System;
using System.Collections.Generic;
using System.Text;

namespace HuH.Communication.Common.Utils
{
    public enum TcpCommand : byte
    {
        HeartbeatRequestCommand = 0x01,
        HeartbeatResponseCommand = 0x02,
        Ping = 0x03,
        Pong = 0x04,

        PrepareAck = 0x05,
        CommitAck = 0x06,

        SlaveAssignment = 0x07,
        CloneAssignment = 0x08,

        CommonRequestCommand =0x31,
        CommonResponseCommand=0x32,

        ServerPushCommand=0x33,
        ServerPushReplayCommand=0x34,
        ServerExcCommand=0x35,

        BadRequest = 0xF0,
        NotHandled = 0xF1,
        NoAnswer = 0xF2,
        IdentifyClient = 0xF5,
        ClientIdentified = 0xF6,
    }
}
