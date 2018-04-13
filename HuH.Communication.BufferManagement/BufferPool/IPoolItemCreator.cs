using System;
using System.Collections.Generic;
using System.Text;

namespace HuH.Communication.BufferManagement.BufferPool
{
    public interface IPoolItemCreator<T>
    {
        IEnumerable<T> Create(int count);
    }
}
