using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HuH.Communication.BufferManagement.BufferPool
{
    public abstract class PoolBase<T> : IPool<T>
    {
        private ConcurrentStack<T> _store;
        private IPoolItemCreator<T> _poolItemCreator;
        private int _availableCount;
        private int _poolerSize;
        private readonly double _flexRate = 0.2;
    
        private int _inExpanding = 0;
        private int _intialPoolSize = 0;
        public int PoolerSize => _poolerSize;

        public int AvailableCount => _availableCount;


        public PoolBase(int poolsize, IPoolItemCreator<T> poolItemCreator)
        {
            _poolItemCreator = poolItemCreator;

            var list = new List<T>(poolsize);

            foreach (var item in poolItemCreator.Create(poolsize))
            {
                list.Add(item);
            }

            _store = new ConcurrentStack<T>(list);
            _intialPoolSize = poolsize;
            _poolerSize = poolsize;
            _availableCount = poolsize;
        }

        public void Clear()
        {
            if (_availableCount == _store.Count)
            {
                _store.Clear();
            }
        }


        public bool Return(T item)
        {
            _store.Push(item);
            Interlocked.Increment(ref _availableCount);

            if (_poolerSize > _intialPoolSize)
            {
                if (_availableCount > _poolerSize * 3 / 4)
                {
                    var decCount = _poolerSize / 2;
                    for (var i = 0; i < decCount; ++i)
                    {
                        _store.TryPop(out T result);
                        Interlocked.Decrement(ref _poolerSize);
                    }
                }
            }
            return true;
        }


        public T Take()
        {
            T item;

            if (_store.TryPop(out item))
            {
                Interlocked.Decrement(ref _availableCount);
                if (_availableCount <= _poolerSize * _flexRate && _inExpanding == 0)
                    ThreadPool.QueueUserWorkItem(w => TryExpand());

                return item;
            }
            if (_inExpanding == 1)
            {
                var spinWait = new SpinWait();

                while (true)
                {
                    spinWait.SpinOnce();

                    if (_store.TryPop(out item))
                    {
                        Interlocked.Decrement(ref _availableCount);
                        return item;
                    }
                    if (_inExpanding != 1)
                        return Take();
                }
            }
            else
            {
                TryExpand();
                return Take();
            }

        }

        bool TryExpand()
        {
            if (Interlocked.CompareExchange(ref _inExpanding, 1, 0) != 0)
                return false;
            Expand();
            _inExpanding = 0;
            return true;
        }


        void Expand()
        {
            var totalCount = _poolerSize;

            foreach (var item in _poolItemCreator.Create(totalCount))
            {
                _store.Push(item);
                Interlocked.Increment(ref _availableCount);
                Interlocked.Increment(ref _poolerSize);
            }
        }



        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }
        protected void Dispose(bool disposing)
        {
            T item;
            if (_store.TryPop(out item))
            {
                (item as IDisposable)?.Dispose();
            }
        }
    }
}
