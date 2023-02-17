using System;
using System.ComponentModel;
using System.Reflection;
using MVVMToolkit.Binding.Localization.Source;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.Generics
{
    public interface IGenericsSolver
    {
        public Type Type { get; }

        public Action Solve(MethodInfo get, object source, MethodInfo set, object target);

        public ILocalizationVariable SolveLocalization(MethodInfo get, object source);

        public ValueChangedBinding SolveValueChanged(Type type, VisualElement element, string key,
            INotifyPropertyChanged binding);

        public Action SolveArraySetElement(PropertyInfo propertyInfo, object source, object[] array, int index);
    }
}