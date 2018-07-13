using HuH.Communication.Common.Components;
using HuH.Communication.Common.Logger;
using System;
using System.Collections.Generic;
using System.Text;

namespace HuH.Communication.Common
{
    /// <summary>
    /// 工作台 默认依赖注入
    /// </summary>
    public class Workbench
    {
        /// <summary>Provides the singleton access instance.
        /// </summary>
        public static Workbench Instance { get; private set; }

        private Workbench() { }

        public static Workbench Create()
        {
            Instance = new Workbench();
            return Instance;
        }

        public Workbench SetDefault<TService, TImplementer>(string serviceName = null, LifeStyle life = LifeStyle.Singleton)
            where TService : class
            where TImplementer : class, TService
        {
            ObjectContainer.Register<TService, TImplementer>(serviceName, life);
            return this;
        }
        public Workbench SetDefault<TService, TImplementer>(TImplementer instance, string serviceName = null)
            where TService : class
            where TImplementer : class, TService
        {
            ObjectContainer.RegisterInstance<TService, TImplementer>(instance, serviceName);
            return this;
        }


        public Workbench BuildContainer()
        {
            ObjectContainer.Build();
            return this;
        }
    }
}
