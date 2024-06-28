using System;
using GameCreator.Runtime.Common;

namespace DaimahouGames.Runtime.Core
{
    public static class TypeUtils
    {
        private static readonly char[] SEPARATOR = {'.'};

        public static string GetNiceName(this Type type)
        {
            return GetNiceName(type.ToString());
        }

        public static string GetNiceName(string type)
        {
            var split = type.Split(SEPARATOR);
            return split.Length > 0 
                ? TextUtils.Humanize(split[^1]) 
                : string.Empty;
        }
    }
}