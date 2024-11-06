using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GM.Dynamic
{
    public sealed class DynamicObject
    {
        private DynamicRefCollection fieldCollection = new();

        public T TryAs<T>() => this.TryAs<T>(DynamicUtil.GetTypeNameCached<T>());
        public T TryAs<T>(string key)
        {
            return this.fieldCollection.Get(key, false).TryAs<T>();
        }

        public ref T As<T>() => ref this.As<T>(DynamicUtil.GetTypeNameCached<T>());
        public ref T As<T>(string key)
        {
            this.fieldCollection.Add(key);
            return ref this.fieldCollection.Get(key).As<T>();
        }

        public bool Is<T>() => Is<T>(DynamicUtil.GetTypeNameCached<T>());
        public bool Is<T>(string key)
        {
            var field = this.fieldCollection.Get(key, false);
            return field.Is<T>();
        }

        public void Reset<T>() => Reset(DynamicUtil.GetTypeNameCached<T>());
        public void Reset(string key)
        {
            this.fieldCollection.Remove(key);
        }

        public DynamicEnumerable<T> Every<T>()
        {
            return new DynamicEnumerable<T>(this.fieldCollection);
        }

        /// <summary>
        /// <para>Can only be found by Every(T)</para>
        /// Every(T) is able to Delete(Reset) the record
        /// </summary>
        public void RecordObject<T>(T value) where T : class
        {
            if (value == null) return;
            this.fieldCollection.GetEmpty().As<T>() = value;
        }

        /// <summary>
        /// <para>Can only be found by Every(T)</para>
        /// Every(T) is able to Delete(Reset) the record
        /// </summary>
        public void RecordStruct<T>(T value) where T : struct
        {
            this.fieldCollection.GetEmpty().As<T>() = value;
        }
    }
}