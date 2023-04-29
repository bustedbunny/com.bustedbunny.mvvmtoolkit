using System;
using System.Reflection;
using Object = UnityEngine.Object;

namespace MVVMToolkit.Binding.Generics
{
    public class AssetSetterFallback<T> : IAssetSetterFallback where T : Object
    {
        public Action<Object> ResolveAssetSetter(PropertyInfo setProp, object target)
        {
            var setter = HelpersGenerics.Set<T>(setProp.GetSetMethod(true), target);
            return o => setter((T)o);
        }
    }

    public interface IAssetSetterFallback
    {
        public Action<Object> ResolveAssetSetter(PropertyInfo setProp, object target);
    }
}