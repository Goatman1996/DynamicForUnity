using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GM.Dynamic
{
    public class DynamicObject
    {
        private DynamicRefCollection fieldCollection = new();

        public T TryAs<T>() => this.TryAs<T>(GetTypeNameCached<T>());
        public T TryAs<T>(string key)
        {
            this.fieldCollection.Add(key);
            return this.fieldCollection.Get(key).TryAs<T>();
        }

        public ref T As<T>() => ref this.As<T>(GetTypeNameCached<T>());
        public ref T As<T>(string key)
        {
            this.fieldCollection.Add(key);
            return ref this.fieldCollection.Get(key).As<T>();
        }

        public bool Is<T>() => Is<T>(GetTypeNameCached<T>());
        public bool Is<T>(string key)
        {
            var field = this.fieldCollection.Get(key, false);
            return field.Is<T>();
        }

        public void Reset<T>() => Reset(GetTypeNameCached<T>());
        public void Reset(string key)
        {
            this.fieldCollection.Remove(key);
        }

        public DynamicEnumerable<T> Every<T>()
        {
            return new DynamicEnumerable<T>(this.fieldCollection);
        }

        private static Dictionary<Type, string> cachedTypeName = new();
        private static string GetTypeNameCached<T>()
        {
            if (!cachedTypeName.ContainsKey(typeof(T)))
            {
                cachedTypeName.Add(typeof(T), typeof(T).Name);
            }
            return cachedTypeName[typeof(T)];
        }
    }
}