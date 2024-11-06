using System;
using System.Collections.Generic;

namespace GM.Dynamic
{
    public static class DynamicUtil
    {
        private static Dictionary<Type, string> cachedTypeName = new();
        public static string GetTypeNameCached<T>()
        {
            return GetTypeNameCached(typeof(T));
        }

        public static string GetTypeNameCached(Type type)
        {
            if (!cachedTypeName.ContainsKey(type))
            {
                cachedTypeName.Add(type, type.FullName);
            }
            return cachedTypeName[type];
        }
    }
}