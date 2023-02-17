using System;
using System.Reflection;

namespace MVVMToolkit.Binding.Generics
{
    public static class HelpersGenerics
    {
        public static Func<T> Get<T>(MethodInfo mi, object obj) => (Func<T>)mi.CreateDelegate(typeof(Func<T>), obj);

        public static Action<T> Set<T>(MethodInfo mi, object obj) =>
            (Action<T>)mi.CreateDelegate(typeof(Action<T>), obj);

        public static Action Solve<T>(MethodInfo get, object source, MethodInfo set, object target)
        {
            var getter = Get<T>(get, source);
            var setter = Set<T>(set, target);
            return () => setter(getter());
        }
    }
}