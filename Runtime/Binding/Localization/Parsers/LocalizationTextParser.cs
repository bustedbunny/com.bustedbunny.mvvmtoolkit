using System;
using System.ComponentModel;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
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
            var table = GetMatchingTable(_stringTables, key);
            var binding = new LocalizedTextBinding(text, bindingContext, key, table, _bindingOperation);
            boundingMap.Add(binding, key);
        }

        private static TableReference GetMatchingTable(LocalizedStringTable[] tables, string key)
        {
            foreach (var table in tables)
            {
                if (table.TableReference.SharedTableData.Contains(key))
                {
                    return table.TableReference;
                }
            }

            throw new BindingException($"No key: {key} found in provided tables.");
        }
    }
}