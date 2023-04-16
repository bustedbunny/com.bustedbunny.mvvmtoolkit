using System;
using System.Collections.Generic;
using System.ComponentModel;
using MVVMToolkit.Binding.Generics;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace MVVMToolkit.Binding.Localization.Source
{
    public class BindingGroup : IVariable, IDisposable
    {
        public LocalizedString Parent { get; }

        public BindingGroup(INotifyPropertyChanged binding, LocalizedString parent)
        {
            Parent = parent;
            _binding = binding;

            Parent.Arguments ??= new List<object>();
            Parent.Arguments.Add(this);

            binding.PropertyChanged += UpdateVariable;
        }

        public void Dispose()
        {
            Parent.Arguments?.Remove(this);

            foreach (var (_, variable) in _variableLookup)
            {
                if (variable is BindingGroupVariable group)
                {
                    group.Clear();
                }
            }

            _variableLookup.Clear();
            _binding.PropertyChanged -= UpdateVariable;
        }

        private readonly INotifyPropertyChanged _binding;

        private readonly Dictionary<string, ILocalizationVariable> _variableLookup = new();

        private void UpdateVariable(object sender, PropertyChangedEventArgs e)
        {
            if (_variableLookup.TryGetValue(e.PropertyName, out var variable)) variable.Set();
        }

        public bool BindGroup(string key, out BindingGroupVariable group)
        {
            group = null;
            if (_variableLookup.TryGetValue(key, out var variable))
            {
                group = (BindingGroupVariable)variable;
                return true;
            }

            var property = PropertyUtility.GetGetProperty(_binding, key);

            group = new(() => new((INotifyPropertyChanged)property.GetValue(_binding), Parent));
            _variableLookup.Add(key, group);
            return true;
        }

        public bool BindVariable(string key, out ILocalizationVariable variable)
        {
            if (_variableLookup.TryGetValue(key, out variable))
            {
                return true;
            }

            var property = PropertyUtility.GetGetProperty(_binding, key);

            variable = BindingUtility.LocalizationVariable(property, _binding);
            _variableLookup.Add(key, variable);
            return true;
        }


        public object GetSourceValue(ISelectorInfo selector) => this;
    }
}