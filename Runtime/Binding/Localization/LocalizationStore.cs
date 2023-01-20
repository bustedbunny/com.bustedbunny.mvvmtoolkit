using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.Localization
{
    public class LocalizationStore : TextBindingStore<LocalizationTextBinding>
    {
        private readonly LocalizedStringTable _stringTable;
        public override char Symbol() => '#';

        public LocalizationStore(object viewModel, LocalizedStringTable stringTable) : base(viewModel)
        {
            _stringTable = stringTable;
        }

        public override void Process(VisualElement element, string key)
        {
            try
            {
                boundingMap.Add(new LocalizationTextBinding((TextElement)element, bindingContext, key, _stringTable),
                    key);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}