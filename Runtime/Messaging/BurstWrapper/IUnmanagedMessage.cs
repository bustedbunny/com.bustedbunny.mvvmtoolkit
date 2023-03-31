using System;
using UnityEngine.Scripting;

namespace MVVMToolkit.Messaging
{
    public interface IUnmanagedMessage { }

    public class Wrapped<T> where T : unmanaged, IUnmanagedMessage
    {
        private static readonly Wrapped<T> MessageInstance = new();

        private static unsafe Wrapped<T> Init(IntPtr ptr)
        {
            MessageInstance.data = *(T*)ptr;
            return MessageInstance;
        }

        [Preserve]
        internal static Func<IntPtr, object> GetDelegate() => Init;

        public T data;
    }
}