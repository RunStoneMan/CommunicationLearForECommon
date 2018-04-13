using System;
using System.Collections.Generic;
using System.Text;

namespace HuH.Communication.BufferManagement.BufferPool
{
    public interface IPool
    {
        /// <summary>
        /// 池大小
        /// </summary>
        int PoolerSize { get; }

        /// <summary>
        /// 池可用大小
        /// </summary>
        int AvailableCount { get; }


        void Clear();
    }


    public interface IPool<T> : IPool
    {
        /// <summary>
        /// 获取池
        /// </summary>
        /// <returns></returns>
        T Take();

        /// <summary>
        /// 元素退回池
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        bool Return(T buffer);
    }


}
