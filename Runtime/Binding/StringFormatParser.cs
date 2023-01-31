using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding
{
    public class StringFormatParser : BindingParser<StringFormatBinding>
    {
        private readonly Action<VisualElement, string> _operation;
        public override char Symbol() => '$';

        public StringFormatParser(INotifyPropertyChanged binding, Action<VisualElement, string> operation) :
            base(binding)
        {
            _operation = operation;
        }

        public override void Process(VisualElement element, string key)
        {
            boundingMap.Add(new StringFormatBinding(bindingContext, element, key, _operation), key);
        }
    }

    public class StringFormatBinding : IElementBinding
    {
        private readonly INotifyPropertyChanged _binding;
        private readonly VisualElement _element;
        private readonly string _key;
        private readonly Action<VisualElement, string> _operation;

        private readonly (string, Func<object>, object)[] _bindings;
        private readonly object[] _return;

        private readonly string _format;

        public StringFormatBinding(INotifyPropertyChanged binding, VisualElement element, string key,
            Action<VisualElement, string> operation)
        {
            _binding = binding;
            _element = element;
            _key = key;
            _operation = operation;

            var formats = BindingUtility.GetFormatKeys(key);
            if (formats is null)
            {
                Debug.LogError($"No variables found in key: {key}.");
                return;
            }

            _bindings = new (string, Func<object>, object)[formats.Length];
            _return = new object[formats.Length];

            for (int i = 0; i < formats.Length; i++)
            {
                var format = formats[i];
                BindingUtility.GetTargetObject(binding, format, out var target, out var propertyName);

                var get = DelegateUtility.GenerateGetter(target, propertyName);

                _bindings[i] = (propertyName, get, target);
                _return[i] = get();
            }

            _format = _key;

            for (var i = 0; i < formats.Length; i++)
            {
                _format = _format.Replace($"{{{formats[i]}}}", $"{{{i}}}");
            }

            UpdateString();

            _binding.PropertyChanged += OnPropertyChanged;
        }

        private void UpdateString()
        {
            _operation(_element, string.Format(_format, _return));
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            for (var i = 0; i < _bindings.Length; i++)
            {
                var (name, func, _) = _bindings[i];
                if (e.PropertyName == name)
                {
                    _return[i] = func();
                    UpdateString();
                    return;
                }
            }
        }

        public void Dispose()
        {
            if (_binding is not null)
                _binding.PropertyChanged -= OnPropertyChanged;
        }
    }
}