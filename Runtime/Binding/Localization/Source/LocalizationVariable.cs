using System;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace MVVMToolkit.Binding.Localization.Source
{
    public interface ILocalizationVariable : IVariableValueChanged
    {
        void Set();
    }


    public class LocalizationVariable<T> : ILocalizationVariable
    {
        private readonly Func<T> _getter;

        public LocalizationVariable(Func<T> getter)
        {
            _getter = getter;
            Set();
        }

        public object GetSourceValue(ISelectorInfo selector) => Value;
        private T _value;

        public void Set()
        {
            Value = _getter();
        }

        private T Value
        {
            get => _value;
            set
            {
                if (value != null && Equals(_value, value))
                    return;

                _value = value;
                ValueChanged?.Invoke(this);
            }
        }

        public event Action<IVariable> ValueChanged;
    }
}