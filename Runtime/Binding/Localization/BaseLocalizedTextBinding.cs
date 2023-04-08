using System;
using System.Collections.Generic;
using System.ComponentModel;
using MVVMToolkit.Binding.Localization.Source;
using UnityEngine.Localization;

namespace MVVMToolkit.Binding.Localization
{
    public abstract class BaseLocalizedTextBinding : IElementBinding
    {
        public abstract void Unbind();
    }

    public class LocalizedTextBinding<T> : BaseLocalizedTextBinding
    {
        private readonly T _boundElement;
        private readonly Action<T, string> _operation;
        private readonly BindingGroup _rootBinding;
        private readonly LocalizedString _localizedString;

        public LocalizedTextBinding(T boundElement, INotifyPropertyChanged binding, LocalizedString ls,
            Action<T, string> operation)
        {
            _operation = operation;
            _boundElement = boundElement;

            _localizedString = ls;

            _rootBinding = new(binding, _localizedString);

            _localizedString.Arguments ??= new List<object>();
            if (!_localizedString.Arguments.Contains(_rootBinding))
                _localizedString.Arguments.Add(_rootBinding);


            _localizedString.StringChanged += StringChanged;

            _localizedString.GetLocalizedStringAsync().Completed += handle => { StringChanged(handle.Result); };
        }

        private void StringChanged(string value) => _operation(_boundElement, value);

        public override void Unbind()
        {
            _rootBinding.ClearBindings();
            _localizedString.Arguments?.Remove(_rootBinding);
        }
    }
}