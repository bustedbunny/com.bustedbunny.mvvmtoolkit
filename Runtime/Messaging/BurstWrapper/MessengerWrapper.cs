using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Scripting;

namespace MVVMToolkit.Messaging
{
    public struct MessengerWrapper
    {
        internal NativeList<byte> data;

        public unsafe void Send<T>(T message) where T : unmanaged, IUnmanagedMessage
        {
            var size = sizeof(T);
            var hash = BurstRuntime.GetHashCode64<T>();

            data.AddRangeNoResize(UnsafeUtility.AddressOf(ref hash), 8);
            data.AddRangeNoResize(UnsafeUtility.AddressOf(ref message), size);
        }

        [Preserve]
        public Parallel AsParallel => new()
        {
            data = data.AsParallelWriter()
        };

        public struct Parallel
        {
            internal NativeList<byte>.ParallelWriter data;

            [Preserve]
            public unsafe void Send<T>(T message) where T : unmanaged, IUnmanagedMessage
            {
                var size = sizeof(T);
                var hash = BurstRuntime.GetHashCode64<T>();

                var length = 8 + size;
                var idx = Interlocked.Add(ref data.ListData->m_length, length) - length;
                var ptr = (byte*)data.Ptr + idx;
                UnsafeUtility.MemCpy(ptr, UnsafeUtility.AddressOf(ref hash), 8);
                UnsafeUtility.MemCpy(ptr + 8, UnsafeUtility.AddressOf(ref message), size);
            }
        }
    }
}