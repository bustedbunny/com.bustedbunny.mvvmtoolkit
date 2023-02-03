using System.Reflection;
using Unity.Collections.LowLevel.Unsafe;

namespace MVVMToolkit.Messaging
{
    public interface IUnmanagedMessage { }

    public class Wrapped<T> where T : unmanaged, IUnmanagedMessage
    {
        internal unsafe Wrapped(Pointer boxedPtr)
        {
            var ptr = (byte*)Pointer.Unbox(boxedPtr);
            data = *(T*)ptr;
        }

        // internal unsafe byte* DataPtr() => (byte*)UnsafeUtility.AddressOf(ref data);

        public T data;
    }
}