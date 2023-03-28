using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace MVVMToolkit.Messaging
{
    public struct MessengerWrapper
    {
        internal NativeList<byte> data;

        public unsafe void Send<T>(T message) where T : unmanaged
        {
            var size = sizeof(T);
            var hash = BurstRuntime.GetHashCode64<T>();

            data.AddRangeNoResize(UnsafeUtility.AddressOf(ref hash), 8);
            data.AddRangeNoResize(UnsafeUtility.AddressOf(ref message), size);
        }
    }
}