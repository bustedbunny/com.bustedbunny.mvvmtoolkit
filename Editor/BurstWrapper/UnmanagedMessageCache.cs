using MVVMToolkit.Editor.TypeSerialization;
using MVVMToolkit.Messaging;
using UnityEditor;

namespace MVVMToolkit.Editor.BurstWrapper
{
    public static class UnmanagedMessageCache
    {
        [InitializeOnLoadMethod]
        private static void Init()
        {
            TypeCaching.CacheTypes(typeof(IUnmanagedMessage));
        }
    }
}