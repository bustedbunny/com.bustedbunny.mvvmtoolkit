using System.Buffers;
using System.Reflection;
using CommunityToolkit.Mvvm.Messaging;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

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

        private static readonly object[] Attributes = { BindingFlags.Instance | BindingFlags.NonPublic };

        public unsafe void Unwrap(StrongReferenceMessenger messenger)
        {
            var ptr = (byte*)data.GetUnsafeReadOnlyPtr();
            var iterator = 0;
            while (iterator < data.Length)
            {
                var hash = UnsafeUtility.AsRef<long>(ptr + iterator);
                var type = TypeManager.GetTypeInfo(hash);


                var dataPtr = ptr + iterator + 8;

                var boxedPointer = Pointer.Box(dataPtr, typeof(byte*));
                var message = type.constructor.Invoke(new[] { boxedPointer });

                iterator += type.size + 8;

                if (message is null)
                {
                    Debug.LogError($"Couldn't unwrap the message of type: {type.dataType.Name}");
                    continue;
                }

                messenger.Send(message);
            }

            data.Clear();
        }
    }
}