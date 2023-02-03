using System;
using Unity.Collections;

namespace MVVMToolkit.Messaging
{
    public class UnmanagedWrapperReference : IDisposable
    {
        private NativeReference<MessengerWrapper> _reference;

        private NativeList<byte> _data;

        public UnmanagedWrapperReference()
        {
            _data = new(1024, Allocator.Persistent);
            var impl = new MessengerWrapper { data = _data };
            _reference = new(impl, Allocator.Persistent);
        }

        public MessengerWrapper Wrapper => _reference.Value;

        public void Dispose()
        {
            _data.Dispose();
            _reference.Dispose();
        }
    }
}