using Autofac;
using HuH.Communication.Common;
using HuH.Communication.Common.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace HuH.Communication.AutoFac
{
   public static class Configurations
    {
        public static Workbench UseAutofac(this Workbench configuration)
        {
            return UseAutofac(configuration, new ContainerBuilder());
        }
        /// <summary>Use Autofac as the object container.
        /// </summary>
        /// <returns></returns>
        public static Workbench UseAutofac(this Workbench configuration, ContainerBuilder containerBuilder)
        {
            ObjectContainer.SetContainer(new AutofacObjectContainer(containerBuilder));
            return configuration;
        }
    }
}
