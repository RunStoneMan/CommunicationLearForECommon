using HuH.Communication.Common.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace HuH.Communication.Transport.Tcp
{
    internal static class Helper
    {
        public static void EatException(Action action)
        {
            try
            {
                action();
            }
            catch (Exception)
            {
            }
        }

        public static T EatException<T>(Func<T> func, T defaultValue = default(T))
        {
            Ensure.NotNull(func, "func");
            try
            {
                return func();
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
    }
}
