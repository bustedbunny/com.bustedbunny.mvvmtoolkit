using System;
using System.ComponentModel;
using MVVMToolkit.Binding.Generics;
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
            boundingMap.Add(new(bindingContext, element, key, _operation), key);
        }
    }

    public class StringFormatBinding : IElementBinding
    {
        private readonly INotifyPropertyChanged _binding;
        private readonly VisualElement _element;
        private readonly Action<VisualElement, string> _operation;

        private readonly (string, Action, object)[] _bindings;
        private readonly object[] _return;

        private readonly string _format;

        public StringFormatBinding(INotifyPropertyChanged binding, VisualElement element, string key,
            Action<VisualElement, string> operation)
        {
            _binding = binding;
            _element = element;
            _operation = operation;

            var formats = ParsingUtility.GetFormatKeys(key);
            if (formats is null)
            {
                Debug.LogError($"No variables found in key: {key}.");
                return;
            }

            _bindings = new (string, Action, object)[formats.Length];
            _return = new object[formats.Length];

            for (int i = 0; i < formats.Length; i++)
            {
                var format = formats[i];

                var setAction = BindingUtility.StringFormatSetAction(_binding, format, _return, i,
                    out var propertyName, out var target);

                _bindings[i] = (propertyName, setAction, target);
                setAction();
            }

            _format = key;

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
            foreach (var (name, setAction, _) in _bindings)
            {
                if (e.PropertyName != name) continue;
                setAction();
                UpdateString();
                return;
            }
        }

        public void Unbind()
        {
            if (_binding is not null)
                _binding.PropertyChanged -= OnPropertyChanged;
        }
    }
}