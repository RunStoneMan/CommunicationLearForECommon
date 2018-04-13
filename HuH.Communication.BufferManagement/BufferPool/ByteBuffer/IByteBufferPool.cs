using System;
using System.Collections.Generic;
using System.Text;

namespace HuH.Communication.BufferManagement.BufferPool.ByteBuffer
{
    public interface IByteBufferPool : IPool<byte[]>
    {
        int BufferSize { get; }
    }
}
