using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using MVVMToolkit.Binding.Localization;
using UnityEngine.Localization;
using Object = UnityEngine.Object;

namespace MVVMToolkit.Binding.Generics
{
    public abstract class ConversionSolver<TSet, TGet> : IConversionSolver where TGet : Object
    {
        public Type GetterType => typeof(TGet);
        public Type SetterType => typeof(TSet);

        protected abstract void AssignValue(Action<TSet> setter, TGet value);

        public LocalizedAssetBinding ResolveBinding(PropertyInfo setProp, object target, LocalizedAssetTable table,
            string key)
        {
            var set = HelpersGenerics.Set<TSet>(setProp.GetSetMethod(true), target);
            var action = new Action<Object>(value => AssignValue(set, Unsafe.As<TGet>(value)));
            return new(table, key, action);
        }
    }
}