using System;
using System.ComponentModel;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.Localization
{
    public class LocalizationTextParser : BindingParser<LocalizedTextBinding>
    {
        private readonly LocalizedStringTable[] _stringTables;
        private readonly Action<VisualElement, string> _bindingOperation;
        public override char Symbol() => '#';

        public LocalizationTextParser(INotifyPropertyChanged viewModel, LocalizedStringTable[] stringTables,
            Action<VisualElement, string> bindingOperation) : base(viewModel)
        {
            _stringTables = stringTables;
            _bindingOperation = bindingOperation;
        }

        public override void Process(VisualElement element, string key)
        {
            var text = (TextElement)element;
            var binding = new LocalizedTextBinding(text, bindingContext, key, _stringTables, _bindingOperation);
            boundingMap.Add(binding, key);
        }
    }
}