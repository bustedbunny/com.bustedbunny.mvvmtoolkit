using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
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

    public abstract class MultiSolver<TGet, TSet> : IMultiSolver where TGet : Object
    {
        public Type GetterType => typeof(TGet);
        public Type SetterType => typeof(TSet);

        protected abstract void AssignValue(Action<TSet> setter, TGet value);

        public Action<Object> ResolveAssetSetter(PropertyInfo setProp, object target)
        {
            var set = HelpersGenerics.Set<TSet>(setProp.GetSetMethod(true), target);
            return value => AssignValue(set, Unsafe.As<TGet>(value));
        }

        public Action Solve(MethodInfo get, object source, MethodInfo set, object target)
        {
            var getter = HelpersGenerics.Get<TGet>(get, source);
            var setter = HelpersGenerics.Set<TSet>(set, target);
            return () => AssignValue(setter, getter());
        }
    }
}