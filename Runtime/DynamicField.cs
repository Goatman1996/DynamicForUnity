using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GM.Dynamic
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DynamicField
    {
        private Type dataType;
        private DataWrapper dataWrapper;

        internal bool IsEmpty => dataType == null;

        private const int SIZE = 56;
        [StructLayout(LayoutKind.Explicit, Size = SIZE)]
        private struct DataWrapper { }

        private static DataWrapper DEFAULT;

        public T TryAs<T>()
        {
            if (!SizeCheck<T>())
            {
                throw new StructOverSizeException($"{typeof(T)} size bigger then {SIZE}");
            }
            if (!Is<T>())
            {
                DEFAULT = default;
                var defaultAddr = Unsafe.AsPointer(ref DEFAULT);
                return Unsafe.AsRef<T>(defaultAddr);
            }
            return As<T>();
        }

        public ref T As<T>()
        {
            if (!SizeCheck<T>())
            {
                throw new StructOverSizeException($"{typeof(T)} size bigger then {SIZE}");
            }
            if (!Is<T>())
            {
                WriteStruct<T>(default);
            }
            var _dataWrapperAddr = Unsafe.AsPointer(ref this.dataWrapper);
            return ref Unsafe.AsRef<T>(_dataWrapperAddr);
        }

        private ref object ReadAsObject()
        {
            var _dataWrapperAddr = Unsafe.AsPointer(ref this.dataWrapper);
            return ref Unsafe.AsRef<object>(_dataWrapperAddr);
        }

        private bool SizeCheck<T>()
        {
            var requireSize = Unsafe.SizeOf<T>();
            return requireSize <= SIZE;
        }

        public bool Is<T>()
        {
            if (this.dataType == null) return false;
            // Class
            if (!this.dataType.IsValueType)
            {
                var obj = this.ReadAsObject();
                return obj is T;
            }

            return this.dataType == typeof(T);
        }

        private void WriteStruct<T>(T data)
        {
            this.dataType = typeof(T);
            var _dataWrapperAddr = Unsafe.AsPointer(ref this.dataWrapper);
            Unsafe.Write(_dataWrapperAddr, data);
        }
    }
}
