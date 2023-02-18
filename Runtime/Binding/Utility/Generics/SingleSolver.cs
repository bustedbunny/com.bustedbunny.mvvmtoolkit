using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using MVVMToolkit.Binding.Localization.Source;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.Generics
{
    public interface ISingleSolver
    {
        public Type Type { get; }

        public Action Solve(MethodInfo get, object source, MethodInfo set, object target);

        public ILocalizationVariable SolveLocalization(MethodInfo get, object source);

        public ValueChangedBinding SolveValueChanged(VisualElement element,
            INotifyPropertyChanged binding, PropertyInfo property);

        public Action SolveArraySetElement(PropertyInfo propertyInfo, object source, object[] array, int index);
    }

    public abstract class SingleSolver<T0> : ISingleSolver
    {
        public Type Type => typeof(T0);

        public Action Solve(MethodInfo get, object source, MethodInfo set, object target)
        {
            return HelpersGenerics.Solve<T0>(get, source, set, target);
        }

        public ILocalizationVariable SolveLocalization(MethodInfo get, object source)
        {
            return new LocalizationVariable<T0>(HelpersGenerics.Get<T0>(get, source));
        }

        public ValueChangedBinding SolveValueChanged(VisualElement element,
            INotifyPropertyChanged binding, PropertyInfo property)
        {
            var get = HelpersGenerics.Get<T0>(property.GetGetMethod(), binding);
            var set = HelpersGenerics.Set<T0>(property.GetSetMethod(), binding);

            return new ValueChangedBinding<T0>((INotifyValueChanged<T0>)element, binding, set, get, property.Name);
        }

        public Action SolveArraySetElement(PropertyInfo propertyInfo, object source, object[] array, int index)
        {
            var get = HelpersGenerics.Get<T0>(propertyInfo.GetGetMethod(), source);

            return () => array[index] = get();
        }
    }
}