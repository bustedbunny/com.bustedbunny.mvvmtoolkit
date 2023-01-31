using System;
using System.Runtime.CompilerServices;

namespace MVVMToolkit.Binding
{
    internal static class Throw
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowNullOrEmpty(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new BindingException("Key cannot be null");
            }
        }
    }

    public class BindingException : Exception
    {
        public BindingException(string error) : base(error) { }
    }
}