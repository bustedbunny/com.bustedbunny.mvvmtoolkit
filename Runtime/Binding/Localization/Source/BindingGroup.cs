using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace MVVMToolkit.Binding.Localization.Source
{
    public class BindingGroup : IVariableGroup, IVariable, IDisposable
    {
        public BindingGroup(INotifyPropertyChanged binding)
        {
            this.binding = binding;

            binding.PropertyChanged += UpdateVariable;
        }

        public void ClearBindings()
        {
            foreach (var (key, variable) in variableLookup)
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


        public bool TryGetValue(string key, out IVariable value)
        {
            if (variableLookup.TryGetValue(key, out var result))
            {
                value = result;
                return true;
            }

            value = default;
            return false;
        }

        public bool BindGroup(string key, out BindingGroupVariable group)
        {
            group = null;
            if (variableLookup.TryGetValue(key, out var variable))
            {
                group = (BindingGroupVariable)variable;
                return true;
            }

            if (!BindUtils.TryGetPropertyGet(binding, key, out var property))
            {
                throw new BindingException($"No property with name: {key} found on type: {binding.GetType().Name}.");
            }

            group = new(() => new((INotifyPropertyChanged)property.GetValue(binding)));
            variableLookup.Add(key, group);
            return true;
        }

        public bool BindVariable(string key, out ILocalizationVariable variable)
        {
            if (variableLookup.TryGetValue(key, out variable))
            {
                return true;
            }

            if (!BindUtils.TryGetPropertyGet(binding, key, out var property))
            {
                throw new BindingException($"No property with name: {key} found on type: {binding.GetType().Name}.");
            }

            variable = BindUtils.LocalizationVariable(property, binding);
            variableLookup.Add(key, variable);
            return true;
        }


        public object GetSourceValue(ISelectorInfo selector) => this;
    }
}