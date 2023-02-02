using System;
using System.ComponentModel;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.Localization
{
    public class LocalizedTextBinding : IElementBinding
    {
        private readonly TextElement _element;
        private readonly Action<VisualElement, string> _operation;
        private readonly LocalizedStringBinding _stringBinding;

        public LocalizedTextBinding(TextElement element, INotifyPropertyChanged binding, string key,
            LocalizedStringTable[] tables,
            Action<VisualElement, string> operation)
        {
            _operation = operation;
            _element = element;

            var table = GetMatchingTable(tables, key);

            LocalizedString s = new(table, key);

            _stringBinding = new(s, binding);

            s.StringChanged += StringChanged;
        }

        private TableReference GetMatchingTable(LocalizedStringTable[] tables, string key)
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

        private void StringChanged(string value) => _operation(_element, value);


        public void Dispose() => _stringBinding.Dispose();
    }
}