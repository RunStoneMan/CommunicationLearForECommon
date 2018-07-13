using HuH.Communication.Transport.Tcp;
using System;
using System.Text;
using Xunit;

namespace HuH.Communication.Transport.TcpTest
{
    public class TcpPackageTest
    {
        [Fact]
        public void FromArraySegmentTest()
        {
            String message = "fsdkfjwokkklalksdijwkfelwkfj";
            TcpPackage tp = new TcpPackage(Common.Utils.TcpCommand.HeartbeatRequestCommand, Encoding.UTF8.GetBytes(message));

            var x = tp.AsArraySegment();

            var m = TcpPackage.FromArraySegment(x);
            Assert.Equal(tp.Id, m.Id);
            Assert.Equal(tp.Command, m.Command);
            Assert.Equal(tp.Body, m.Body);
        }
    }
}
