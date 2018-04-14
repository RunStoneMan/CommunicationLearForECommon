using HuH.Communication.Common.Components;
using HuH.Communication.Common.Logger;
using System;
using System.Collections.Generic;
using System.Text;

namespace HuH.Communication.Common.Extensions
{
    public static class WorkbenchExtensions
    {
        public static Workbench RegisterCommonComponents(this Workbench wk)
        {
            wk.SetDefault<ILoggerFactory, Log4NetLoggerFactory>();
            return wk;
        }
        public static Workbench RegisterUnhandledExceptionHandler(this Workbench wk)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var logger = ObjectContainer.Resolve<ILoggerFactory>().Create(wk.GetType().FullName);
                logger.ErrorFormat("Unhandled exception: {0}", e.ExceptionObject);
            };
            return wk;
        }
    }
}
