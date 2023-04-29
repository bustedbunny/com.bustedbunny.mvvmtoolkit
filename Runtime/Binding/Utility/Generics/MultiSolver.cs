using System;
using System.Reflection;
using Object = UnityEngine.Object;

namespace MVVMToolkit.Binding.Generics
{
    public interface IMultiSolver
    {
        public Type GetterType { get; }
        public Type SetterType { get; }

        public Action<Object> ResolveAssetSetter(PropertyInfo setProp, object target);
        public Action Solve(MethodInfo get, object source, MethodInfo set, object target);
    }

    public abstract class MultiSolver<TGet, TSet> : IMultiSolver where TGet : Object, TSet where TSet : Object
    {
        public Type GetterType => typeof(TGet);
        public Type SetterType => typeof(TSet);

        public Action<Object> ResolveAssetSetter(PropertyInfo setProp, object target)
        {
            var set = HelpersGenerics.Set<TSet>(setProp.GetSetMethod(true), target);
            return value => set((TSet)value);
        }

        public Action Solve(MethodInfo get, object source, MethodInfo set, object target)
        {
            var getter = HelpersGenerics.Get<TGet>(get, source);
            var setter = HelpersGenerics.Set<TSet>(set, target);
            return () =>
            {
                var value = getter();
                setter(value);
            };
        }
    }
}