using MVVMToolkit.Binding.Custom;
using MVVMToolkit.Binding.Generics;
using MVVMToolkit.Editor.TypeSerialization;
using UnityEditor;

namespace MVVMToolkit.Editor.Binding
{
    internal static class TypeCache
    {
        [InitializeOnLoadMethod]
        private static void Init()
        {
            // Custom
            TypeCaching.CacheTypes(typeof(ICustomBinder));

            // Solvers
            TypeCaching.CacheTypes(typeof(ISingleSolver));
            TypeCaching.CacheTypes(typeof(IMultiSolver));
        }
    }
}