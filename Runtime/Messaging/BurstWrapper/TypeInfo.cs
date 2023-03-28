using System;

namespace MVVMToolkit.Messaging
{
    public class TypeInfo
    {
        public TypeInfo(int size, Type wrapperType, Type dataType, Func<IntPtr, object> constructor)
        {
            this.size = size;
            this.wrapperType = wrapperType;
            this.dataType = dataType;
            this.constructor = constructor;
        }

        public readonly int size;
        public readonly Type wrapperType;
        public readonly Type dataType;
        public readonly Func<IntPtr, object> constructor;
    }
}