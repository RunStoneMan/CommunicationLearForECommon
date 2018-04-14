﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HuH.Communication.Common.Logger
{
    public interface ILoggerFactory
    {
        /// <summary>Create a logger with the given logger name.
        /// </summary>
        ILogger Create(string name);
        /// <summary>Create a logger with the given type.
        /// </summary>
        ILogger Create(Type type);
    }
}
