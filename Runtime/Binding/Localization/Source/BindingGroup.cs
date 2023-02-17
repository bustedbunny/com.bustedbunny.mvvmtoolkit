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
            this.binding = binding;

            binding.PropertyChanged += UpdateVariable;
        }

        public void ClearBindings()
        {
            foreach (var (_, variable) in variableLookup)
            {
                if (variable is BindingGroupVariable group)
                {
                    group.Clear();
                }
            }

            variableLookup.Clear();
        }

        public void Dispose()
        {
            ClearBindings();
            binding.PropertyChanged -= UpdateVariable;
        }

        public INotifyPropertyChanged binding;

        private readonly Dictionary<string, ILocalizationVariable> variableLookup = new();

        private void UpdateVariable(object sender, PropertyChangedEventArgs e)
        {
            if (variableLookup.TryGetValue(e.PropertyName, out var variable)) variable.Set();
        }

        public bool BindGroup(string key, out BindingGroupVariable group)
        {
            group = null;
            if (variableLookup.TryGetValue(key, out var variable))
            {
                group = (BindingGroupVariable)variable;
                return true;
            }

            var property = PropertyUtility.GetGetProperty(binding, key);

            group = new(() => new((INotifyPropertyChanged)property.GetValue(binding), Parent));
            variableLookup.Add(key, group);
            return true;
        }

        public bool BindVariable(string key, out ILocalizationVariable variable)
        {
            if (variableLookup.TryGetValue(key, out variable))
            {
                return true;
            }

            var property = PropertyUtility.GetGetProperty(binding, key);

            variable = GenericsUtility.LocalizationVariable(property, binding);
            variableLookup.Add(key, variable);
            return true;
        }


        public object GetSourceValue(ISelectorInfo selector) => this;
    }
}