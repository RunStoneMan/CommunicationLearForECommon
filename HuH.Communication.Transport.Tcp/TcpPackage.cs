using HuH.Communication.Common.Utils;
using HuH.Communication.Transport.Tcp.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HuH.Communication.Transport.Tcp
{
    public enum OperationResult
    {
        Success = 0,
        PrepareTimeout = 1,
        CommitTimeout = 2,
        ForwardTimeout = 3,
        WrongExpectedVersion = 4,
        StreamDeleted = 5,
        InvalidTransaction = 6,
        AccessDenied = 7,
        Error=9
    }

    public class TcpPackage
    {
        private static long _sequence;
        public string Id { get; set; }

        public OperationResult GetOperationResult { set; get; }

        public readonly TcpCommand Command;

        public long Sequence { get; set; }
        public byte[] Body { get; set; }
        public DateTime CreatedTime { get; set; }
        public IDictionary<string, string> Header { get; set; }

        public TcpPackage() { }

        public TcpPackage(TcpCommand command, byte[] body, IDictionary<string, string> header = null) :
            this(ObjectId.GenerateNewStringId(), command, Interlocked.Increment(ref _sequence), body, DateTime.Now, OperationResult.Success, header)
        { }

        public TcpPackage(TcpCommand command, long sequence, OperationResult result, byte[] body, IDictionary<string, string> header = null) :
         this(ObjectId.GenerateNewStringId(), command, sequence, body, DateTime.Now, result, header)
        { }

        public TcpPackage(TcpCommand command, long sequence,byte[] body, IDictionary<string, string> header = null) : 
            this(ObjectId.GenerateNewStringId(), command, sequence, body, DateTime.Now, OperationResult.Success, header) { }
        public TcpPackage(TcpCommand command,byte[] body, OperationResult operationResult, IDictionary<string, string> header = null ) : 
            this(ObjectId.GenerateNewStringId(), command, Interlocked.Increment(ref _sequence), body, DateTime.Now, operationResult, header) { }
        public TcpPackage(string id, TcpCommand command, long sequence, byte[] body, DateTime createdTime, OperationResult operationResult, IDictionary<string, string> header)
        {
            Id = id;
            Command = command;
            Sequence = sequence;
            GetOperationResult = operationResult;
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
            var resutl = (OperationResult)ByteUtil.DecodeInt(data.ToArray(), srcOffset, out srcOffset);
            var bodyLength = data.Count - srcOffset;
            var body = new byte[bodyLength];

            Buffer.BlockCopy(data.ToArray(), srcOffset, body, 0, bodyLength);

            return new TcpPackage(id, command,sequence, body, createdTime, resutl, header);
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
            var result = BitConverter.GetBytes((int)GetOperationResult);
            return ByteUtil.Combine(
                IdLengthBytes,
                IdBytes,
                sequenceBytes,
                command,
                createdTimeBytes,
                headerLengthBytes,
                headerBytes,
                result,
                Body);
        }
    }
}
