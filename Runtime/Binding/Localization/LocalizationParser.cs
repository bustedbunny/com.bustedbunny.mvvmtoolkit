using System;
using System.ComponentModel;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.Localization
{
    public class LocalizationParser : BindingParser<LocalizedTextBinding>
    {
        private readonly LocalizedStringTable _stringTable;
        private readonly Action<VisualElement, string> _bindingOperation;
        public override char Symbol() => '#';

        public LocalizationParser(INotifyPropertyChanged viewModel, LocalizedStringTable stringTable,
            Action<VisualElement, string> bindingOperation) : base(viewModel)
        {
            _stringTable = stringTable;
            _bindingOperation = bindingOperation;
        }

        public override void Process(VisualElement element, string key)
        {
            var text = (TextElement)element;
            var binding = new LocalizedTextBinding(text, bindingContext, key, _stringTable, _bindingOperation);
            boundingMap.Add(binding, key);
        }
    }
}