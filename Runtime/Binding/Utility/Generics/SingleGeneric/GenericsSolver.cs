using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using MVVMToolkit.Binding.Localization.Source;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.Generics
{
    public abstract class GenericsSolver<T0> : IGenericsSolver
    {
        public Type Type => typeof(T0);
        private static Func<T> Get<T>(MethodInfo mi, object obj) => (Func<T>)mi.CreateDelegate(typeof(Func<T>), obj);

        private static Action<T> Set<T>(MethodInfo mi, object obj) =>
            (Action<T>)mi.CreateDelegate(typeof(Action<T>), obj);

        private Action Solve<T>(MethodInfo get, object source, MethodInfo set, object target)
        {
            var getter = Get<T>(get, source);
            var setter = Set<T>(set, target);
            return () => setter(getter());
        }

        public Action Solve(MethodInfo get, object source, MethodInfo set, object target)
        {
            return Solve<T0>(get, source, set, target);
        }

        public ILocalizationVariable SolveLocalization(MethodInfo get, object source)
        {
            return new LocalizationVariable<T0>(Get<T0>(get, source));
        }

        public ValueChangedBinding SolveValueChanged(Type type, VisualElement element, string key,
            INotifyPropertyChanged binding)
        {
            BindingUtility.GetTargetObject(binding, key, out var target, out var propertyName);

            var property = PropertyUtility.GetGetSetProperty(target, propertyName);
            Debug.Assert(property != null, nameof(property) + " != null");
            var get = Get<T0>(property.GetGetMethod(), target);
            var set = Set<T0>(property.GetSetMethod(), target);

            return new ValueChangedBinding<T0>((INotifyValueChanged<T0>)element, (INotifyPropertyChanged)target, set,
                get, propertyName);
        }

        public Action SolveArraySetElement(PropertyInfo propertyInfo, object source, object[] array, int index)
        {
            var get = Get<T0>(propertyInfo.GetGetMethod(), source);

            return () => array[index] = get();
        }
    }
}