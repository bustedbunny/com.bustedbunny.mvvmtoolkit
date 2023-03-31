using System;
using CommunityToolkit.Mvvm.Messaging;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace MVVMToolkit.Messaging
{
    public class WrapperReference : IDisposable
    {
        private readonly StrongReferenceMessenger _messenger;

        private NativeList<byte> _data;

        public WrapperReference(StrongReferenceMessenger messenger)
        {
            _messenger = messenger;
            _data = new(1024, Allocator.Persistent);
        }

        /// <summary>
        /// Unwraps all written to buffer messages. Not thread safe.
        /// </summary>
        public unsafe void Unwrap()
        {
            var ptr = (byte*)_data.GetUnsafeReadOnlyPtr();
            var iterator = 0;
            while (iterator < _data.Length)
            {
                var hash = UnsafeUtility.AsRef<long>(ptr + iterator);
                var type = TypeManager.GetTypeInfo(hash);


                var dataPtr = ptr + iterator + 8;

                var message = type.constructor((IntPtr)dataPtr);

                iterator += type.size + 8;

                if (message is null)
                {
                    Debug.LogError($"Couldn't unwrap the message of type: {type.dataType.Name}");
                    continue;
                }

                _messenger.SendTyped(message, type.wrapperType);
            }

            _data.Clear();
        }

        public MessengerWrapper Wrapper => new() { data = _data };

        public void Dispose() => _data.Dispose();
    }
}