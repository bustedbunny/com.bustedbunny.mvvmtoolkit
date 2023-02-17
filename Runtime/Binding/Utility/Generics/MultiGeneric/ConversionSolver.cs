using System;
using System.Reflection;
using MVVMToolkit.Binding.Localization;
using UnityEngine.Localization;
using Object = UnityEngine.Object;

namespace MVVMToolkit.Binding.Generics
{
    public abstract class ConversionSolver<TSet, TGet> : IConversionSolver where TGet : Object
    {
        public Type SetType => typeof(TSet);

        public bool CanSolve(Type set, Type get)
        {
            return typeof(TSet) == set && typeof(TGet) == get;
        }

        protected abstract void AssignValue(Action<TSet> setter, TGet value);

        public LocalizedAssetBinding ResolveBinding(PropertyInfo setProp, object target, LocalizedAssetTable table,
            string key)
        {
            var set = HelpersGenerics.Set<TSet>(setProp.GetSetMethod(true), target);
            var action = new Action<Object>(value => AssignValue(set, (TGet)value));
            return new LocalizedAssetBinding<TGet>(table, key, action);
        }
    }
}