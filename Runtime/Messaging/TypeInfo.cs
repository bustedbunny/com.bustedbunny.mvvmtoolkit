using System;
using System.Reflection;

namespace MVVMToolkit.Messaging
{
    public class TypeInfo
    {
        public TypeInfo(int size, Type wrapperType, Type dataType, ConstructorInfo constructor)
        {
            this.size = size;
            this.wrapperType = wrapperType;
            this.dataType = dataType;
            this.constructor = constructor;
        }

        public readonly int size;
        public readonly Type wrapperType;
        public readonly Type dataType;
        public readonly ConstructorInfo constructor;
    }
}