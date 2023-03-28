using MVVMToolkit.Binding.Generics;
using MVVMToolkit.Editor.TypeSerialization;
using UnityEditor;

namespace MVVMToolkit.Editor.Binding
{
    public static class SolverTypeCache
    {
        [InitializeOnLoadMethod]
        private static void Init()
        {
            TypeCaching.CacheTypes(typeof(ISingleSolver));
            TypeCaching.CacheTypes(typeof(IMultiSolver));
        }
    }
}