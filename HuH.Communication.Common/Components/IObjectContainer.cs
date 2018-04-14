using System;
using System.Collections.Generic;
using System.Text;

namespace HuH.Communication.Common.Components
{
    public interface IObjectContainer
    {
       
        void Build();
       
        void RegisterType(Type implementationType, string serviceName = null, LifeStyle life = LifeStyle.Singleton);
      
        void RegisterType(Type serviceType, Type implementationType, string serviceName = null, LifeStyle life = LifeStyle.Singleton);
     
        void Register<TService, TImplementer>(string serviceName = null, LifeStyle life = LifeStyle.Singleton)
            where TService : class
            where TImplementer : class, TService;
      
        void RegisterInstance<TService, TImplementer>(TImplementer instance, string serviceName = null)
            where TService : class
            where TImplementer : class, TService;
      
        TService Resolve<TService>() where TService : class;
      
        object Resolve(Type serviceType);
  
        bool TryResolve<TService>(out TService instance) where TService : class;
     
        bool TryResolve(Type serviceType, out object instance);

        TService ResolveNamed<TService>(string serviceName) where TService : class;
  
        object ResolveNamed(string serviceName, Type serviceType);
  
        bool TryResolveNamed(string serviceName, Type serviceType, out object instance);
    }
}
