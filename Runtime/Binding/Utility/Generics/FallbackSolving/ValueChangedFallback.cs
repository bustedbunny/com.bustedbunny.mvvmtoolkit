using System.ComponentModel;
using System.Reflection;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.Generics
{
    public class ValueChangedFallback<T> : IValueChangedFallback
    {
        public ValueChangedBinding SolveValueChanged(VisualElement element,
            INotifyPropertyChanged binding, PropertyInfo property)
        {
            var get = HelpersGenerics.Get<T>(property.GetGetMethod(), binding);
            var set = HelpersGenerics.Set<T>(property.GetSetMethod(), binding);

            return new ValueChangedBinding<T>((INotifyValueChanged<T>)element, binding, set, get, property.Name);
        }
    }

    public interface IValueChangedFallback
    {
        public ValueChangedBinding SolveValueChanged(VisualElement element,
            INotifyPropertyChanged binding, PropertyInfo property);
    }
}