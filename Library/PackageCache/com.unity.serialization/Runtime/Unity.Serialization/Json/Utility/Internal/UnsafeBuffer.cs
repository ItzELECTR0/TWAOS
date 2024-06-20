using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Serialization
{
    readonly unsafe struct UnsafeBuffer<T> where T : unmanaged
    {
        [NativeDisableUnsafePtrRestriction] 
        public readonly T* Buffer;
        public readonly int Length;

        public UnsafeBuffer(T* buffer, int length)
        {
            Buffer = buffer;
            Length = length;
        }
    }
}