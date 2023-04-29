using System;
using System.Reflection;

namespace MVVMToolkit.Binding.Generics
{
    internal class SingleFallback<TSource, TTarget, TType> : IFallbackSolver
    {
        public Action<object, object> Solve(MethodInfo get, MethodInfo set)
        {
            var getter = (Func<TSource, TType>)Delegate.CreateDelegate(typeof(Func<TSource, TType>), get);
            var setter = (Action<TTarget, TType>)Delegate.CreateDelegate(typeof(Action<TTarget, TType>), set);

            return (source, target) =>
            {
                var value = getter((TSource)source);
                setter((TTarget)target, value);
            };
        }

        public Action Solve(MethodInfo get, object source, MethodInfo set, object target)
        {
            var getter = (Func<TType>)Delegate.CreateDelegate(typeof(Func<TType>), source, get);
            var setter = (Action<TType>)Delegate.CreateDelegate(typeof(Action<TType>), target, set);

            return () =>
            {
                var value = getter();
                setter(value);
            };
        }
    }
}