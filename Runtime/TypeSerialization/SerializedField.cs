using System;
using System.Collections.Generic;

namespace MVVMToolkit.TypeSerialization
{
    [Serializable]
    public class SerializedField
    {
        public string fullTypeName;
        public List<string> fieldNames;
    }
}