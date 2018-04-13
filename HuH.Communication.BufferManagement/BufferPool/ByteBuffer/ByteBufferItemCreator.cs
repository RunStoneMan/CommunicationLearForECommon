using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HuH.Communication.BufferManagement.BufferPool.ByteBuffer
{
    public class ByteBufferItemCreator : IPoolItemCreator<byte[]>
    {
        private int _bufferSize;

        public ByteBufferItemCreator(int bufferSize)
        {
            _bufferSize = bufferSize;
        }

        public IEnumerable<byte[]> Create(int count)
        {
            return new ByteBufferItemEnumerable(_bufferSize, count);
        }
    }

    public class ByteBufferItemEnumerable : IEnumerable<byte[]>
    {
        private int _bufferSize;
        private int _count;

        public ByteBufferItemEnumerable(int bufferSize, int count)
        {
            _bufferSize = bufferSize;
            _count = count;
        }

        public IEnumerator<byte[]> GetEnumerator()
        {
            int count = _count;

            for (int i = 0; i < count; i++)
            {
                yield return new byte[_bufferSize];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
