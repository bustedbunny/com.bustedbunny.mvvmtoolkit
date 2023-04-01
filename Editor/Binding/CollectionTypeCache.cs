using MVVMToolkit.Binding.CollectionBinding.Generics;
using MVVMToolkit.Binding.Custom;
using MVVMToolkit.Binding.Generics;
using MVVMToolkit.Editor.TypeSerialization;
using UnityEditor;

namespace MVVMToolkit.Editor.Binding
{
    internal static class CollectionTypeCache
    {
        [InitializeOnLoadMethod]
        private static void Init()
        {
            // Custom
            TypeCaching.CacheTypes(typeof(ICustomBinder));

            // Collections
            TypeCaching.CacheTypes(typeof(IItemCollectionBinder));
            TypeCaching.CacheTypes(typeof(ICollectionBinder));

            // Solvers
            TypeCaching.CacheTypes(typeof(ISingleSolver));
            TypeCaching.CacheTypes(typeof(IMultiSolver));
        }
    }
}