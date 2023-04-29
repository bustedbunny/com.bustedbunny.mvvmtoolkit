using System;
using System.Reflection;

namespace MVVMToolkit.Binding.Generics
{
    internal class ConversionFallback<TSource, TGet, TTarget, TSet> : IFallbackSolver
    {
        public Action<object, object> Solve(MethodInfo get, MethodInfo set)
        {
            var getter = (Func<TSource, TGet>)Delegate.CreateDelegate(typeof(Func<TSource, TGet>), get);
            var setter = (Action<TTarget, TSet>)Delegate.CreateDelegate(typeof(Action<TTarget, TSet>), set);

            var converter = FallbackUtility.GetConverter<TGet, TSet>();

            return (source, target) =>
            {
                var value = getter((TSource)source);
                setter((TTarget)target, converter(value));
            };
        }

        public Action Solve(MethodInfo get, object source, MethodInfo set, object target)
        {
            var getter = (Func<TGet>)Delegate.CreateDelegate(typeof(Func<TGet>), source, get);
            var setter = (Action<TSet>)Delegate.CreateDelegate(typeof(Action<TSet>), target, set);

            var converter = FallbackUtility.GetConverter<TGet, TSet>();

            return () =>
            {
                var value = getter();
                setter(converter(value));
            };
        }
    }
}