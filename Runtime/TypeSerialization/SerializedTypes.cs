using System;
using System.Collections.Generic;

namespace MVVMToolkit.TypeSerialization
{
    [Serializable]
    public class SerializedTypes
    {
        public string assemblyName;
        public List<string> fullTypeNames;
    }
}