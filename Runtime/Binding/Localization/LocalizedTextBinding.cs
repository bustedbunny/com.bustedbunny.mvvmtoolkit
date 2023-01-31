using System;
using System.ComponentModel;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.Localization
{
    public class LocalizedTextBinding : IElementBinding
    {
        private readonly TextElement _element;
        private readonly Action<VisualElement, string> _operation;
        private readonly LocalizedStringBinding _stringBinding;

        public LocalizedTextBinding(TextElement element, INotifyPropertyChanged binding, string key,
            LocalizedStringTable table,
            Action<VisualElement, string> operation)
        {
            _operation = operation;
            _element = element;

            LocalizedString s = new(table.TableReference, key);

            _stringBinding = new(s, binding);

            s.StringChanged += StringChanged;
        }

        private void StringChanged(string value) => _operation(_element, value);


        public void Dispose() => _stringBinding.Dispose();
    }
}