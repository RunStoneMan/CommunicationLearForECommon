using HuH.Communication.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HuH.Communication.Transport.Tcp
{

    [Flags]
    public enum TcpFlags : byte
    {
        None = 0x00,
        Authenticated = 0x01,
        TrustedWrite = 0x02
    }


    public class TcpPackage
    {


        public const int CommandOffset = 0;



        private static long _sequence;
        public string Id { get; set; }
        public readonly TcpCommand Command;

        public long Sequence { get; set; }
        public byte[] Body { get; set; }
        public DateTime CreatedTime { get; set; }
        public IDictionary<string, string> Header { get; set; }

        public TcpPackage() { }
        public TcpPackage(TcpCommand command, byte[] body, IDictionary<string, string> header = null) : this(ObjectId.GenerateNewStringId(), command, Interlocked.Increment(ref _sequence), body, DateTime.Now, header) { }
        public TcpPackage(string id, TcpCommand command, long sequence, byte[] body, DateTime createdTime, IDictionary<string, string> header)
        {
            Id = id;
            Command = command;
            Sequence = sequence;
            Body = body;
            Header = header;
            CreatedTime = createdTime;
        }


        public override string ToString()
        {
            var createdTime = CreatedTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var bodyLength = 0;
            if (Body != null)
            {
                bodyLength = Body.Length;
            }
            var header = string.Empty;
            if (Header != null && Header.Count > 0)
            {
                header = string.Join(",", Header.Select(x => string.Format("{0}:{1}", x.Key, x.Value)));
            }
            return string.Format("[Id:{0}, Type:{1}, Command:{2}, Sequence:{3}, CreatedTime:{4}, BodyLength:{5}, Header: [{6}]]",
                Id, Command, Sequence, createdTime, bodyLength, header);
        }


        public static TcpPackage FromArraySegment(ArraySegment<byte> data)
        {

            var srcOffset = 0;
            var id = ByteUtil.DecodeString(data.ToArray(), srcOffset, out srcOffset);
            var sequence = ByteUtil.DecodeLong(data.ToArray(), srcOffset, out srcOffset);
            var command = (TcpCommand)ByteUtil.DecodeByte(data.ToArray(), srcOffset, out srcOffset);
            var createdTime = ByteUtil.DecodeDateTime(data.ToArray(), srcOffset, out srcOffset);
            var headerLength = ByteUtil.DecodeInt(data.ToArray(), srcOffset, out srcOffset);
            var header = HeaderUtil.DecodeHeader(data.ToArray(), srcOffset, out srcOffset);
            var bodyLength = data.Count - srcOffset;
            var body = new byte[bodyLength];

            Buffer.BlockCopy(data.ToArray(), srcOffset, body, 0, bodyLength);

            return new TcpPackage(id, command, sequence, body, createdTime, header);
        }

        public ArraySegment<byte> AsArraySegment()
        {
            return new ArraySegment<byte>(AsByteArray());
        }

        private byte[] AsByteArray()
        {
            byte[] IdBytes;
            byte[] IdLengthBytes;
            ByteUtil.EncodeString(Id, out IdLengthBytes, out IdBytes);

            var sequenceBytes = BitConverter.GetBytes(Sequence);
            var command =new byte[] { (byte)Command };
            var createdTimeBytes = ByteUtil.EncodeDateTime(CreatedTime);
            var headerBytes = HeaderUtil.EncodeHeader(Header);
            var headerLengthBytes = BitConverter.GetBytes(headerBytes.Length);

            return ByteUtil.Combine(
                IdLengthBytes,
                IdBytes,
                sequenceBytes,
                command,
                createdTimeBytes,
                headerLengthBytes,
                headerBytes,
                Body);
        }
    }
}
