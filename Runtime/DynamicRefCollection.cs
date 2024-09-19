using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GM.Dynamic
{
    internal class DynamicRefCollection
    {
        private static DynamicField DefaultField;
        private DynamicField[] dataList = new DynamicField[8];
        private Dictionary<string, int> dataIndexDict = new();

        internal int Length { get { return dataList.Length; } }

        internal ref DynamicField GetAt(int index)
        {
            return ref dataList[index];
        }

        internal ref DynamicField Get(string key, bool check = true)
        {
            if (this.dataIndexDict.ContainsKey(key))
            {
                var index = this.dataIndexDict[key];
                return ref this.dataList[index];
            }
            else if (check)
            {
                throw new Exception($"[DynamicRefCollection] Get {key} not found!");
            }
            DefaultField = default;
            return ref DefaultField;
        }

        internal void Add(string key)
        {
            if (dataIndexDict.ContainsKey(key))
            {
                return;
            }
            int emptyIndex = FindFirstEmptyField();
            dataIndexDict[key] = emptyIndex;
        }

        internal void Remove(string key)
        {
            if (!dataIndexDict.ContainsKey(key))
            {
                return;
            }
            int index = dataIndexDict[key];
            dataList[index] = new DynamicField();
            dataIndexDict.Remove(key);
        }

        private int FindFirstEmptyField()
        {
            int emptyIndex = -1;
            for (int i = 0; i < dataList.Length; i++)
            {
                if (dataList[i].IsEmpty)
                {
                    emptyIndex = i;
                    break;
                }
            }

            var currentLength = dataList.Length;
            if (emptyIndex == -1)
            {
                Array.Resize(ref dataList, currentLength * 2);
                emptyIndex = currentLength;
            }
            return emptyIndex;
        }
    }

    public struct DynamicEnumerable<T>
    {
        internal DynamicRefCollection collector;
        internal DynamicEnumerable(DynamicRefCollection collector)
        {
            this.collector = collector;
        }

        public DynamicEnumerator<T> GetEnumerator()
        {
            return new DynamicEnumerator<T>(collector);
        }
    }

    public struct DynamicEnumerator<T> : IEnumerator<T>
    {
        internal DynamicRefCollection collector;

        internal DynamicEnumerator(DynamicRefCollection collector)
        {
            this.collector = collector;
            index = -1;
        }

        public T Current
        {
            get
            {
                var field = collector.GetAt(index);
                return field.As<T>();
            }
        }

        public ref T RefCurrent
        {
            get => ref collector.GetAt(index).As<T>();
        }
        object IEnumerator.Current => throw new NotImplementedException();

        private int index;

        public void Dispose()
        {

        }

        public bool MoveNext()
        {
            index++;
            for (int i = index; i < this.collector.Length; i++)
            {
                var value = this.collector.GetAt(i);
                if (value.Is<T>())
                {
                    index = i;
                    return true;
                }
            }
            return false;
        }

        public void Reset()
        {
            index = -1;
        }
    }
}