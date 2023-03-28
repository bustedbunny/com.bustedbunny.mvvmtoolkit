using System;
using UnityEngine.Scripting;

namespace MVVMToolkit.Messaging
{
    public interface IUnmanagedMessage { }

    public class Wrapped<T> where T : unmanaged, IUnmanagedMessage
    {
        private unsafe Wrapped(byte* ptr) => data = *(T*)ptr;

        private static unsafe Wrapped<T> Init(IntPtr ptr)
        {
            return new((byte*)ptr);
        }

        [Preserve]
        internal static Func<IntPtr, object> GetDelegate() => Init;

        public T data;
    }
}