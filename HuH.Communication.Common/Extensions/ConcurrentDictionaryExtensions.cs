﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace HuH.Communication.Common.Extensions
{
    public static class ConcurrentDictionaryExtensions
    {
        public static bool Remove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key)
        {
            TValue value;
            return dict.TryRemove(key, out value);
        }
    }
}
