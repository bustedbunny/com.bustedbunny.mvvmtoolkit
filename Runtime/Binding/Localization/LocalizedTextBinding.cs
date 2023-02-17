using System;
using System.Collections.Generic;
using System.ComponentModel;
using MVVMToolkit.Binding.Localization.Source;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.Localization
{
    public class LocalizedTextBinding : IElementBinding
    {
        private readonly TextElement _element;
        private readonly Action<VisualElement, string> _operation;
        private readonly BindingGroup _rootBinding;
        private readonly LocalizedString _localizedString;

        public LocalizedTextBinding(TextElement element, INotifyPropertyChanged binding, string key,
            TableReference table,
            Action<VisualElement, string> operation)
        {
            _operation = operation;
            _element = element;

            _localizedString = new(table, key);

            _rootBinding = new(binding, _localizedString);

            _localizedString.Arguments ??= new List<object>();
            if (!_localizedString.Arguments.Contains(_rootBinding))
                _localizedString.Arguments.Add(_rootBinding);


            _localizedString.StringChanged += StringChanged;
        }

        private void StringChanged(string value) => _operation(_element, value);

        public void Unbind()
        {
            _rootBinding.ClearBindings();
            _localizedString.Arguments?.Remove(_rootBinding);
        }
    }
}