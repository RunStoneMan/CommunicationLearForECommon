using System;
using System.Collections.Generic;
using System.Text;

namespace HuH.Communication.BufferManagement.BufferPool.ByteBuffer
{
    public class ByteBuffer : PoolBase<byte[]>, IByteBufferPool
    {

        public int BufferSize { get; private set; }

        public ByteBuffer(int bufferSize, int initialCount)
            : base(initialCount, new ByteBufferItemCreator(bufferSize))
        {
            BufferSize = bufferSize;
        }
    }
}
