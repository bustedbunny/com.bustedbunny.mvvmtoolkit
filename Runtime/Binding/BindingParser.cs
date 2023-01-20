using System;
using System.Collections.Generic;
using MVVMToolkit.Binding.Localization;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding
{
    public class BindingParser : IDisposable
    {
        private readonly VisualElement _rootVisualElement;

        private ClickStore _clickStore;

        private readonly List<IBindingStore> _bindingTextStores = new();

        private readonly List<IBindingStore> _bindingPathStores = new();

        public BindingParser(object model, VisualElement root, LocalizedStringTable table)
        {
            _rootVisualElement = root;

            if (table is not null && !table.IsEmpty)
            {
                _bindingTextStores.Add(new LocalizationStore(model, table));
            }

            _bindingTextStores.Add(new ClickStore(model));

            _bindingPathStores.Add(new ValueChangedStore(model));

            ParseBindings();
        }

        private void ParseBindings()
        {
            ParseText();
            ParsePath();
        }

        private void ParseText()
        {
            _rootVisualElement.Query<TextElement>().ForEach((elem) =>
            {
                if (string.IsNullOrEmpty(elem.text)) return;

                var bindings = elem.text.Replace(" ", "").Split(';');

                foreach (var bindingStore in _bindingTextStores)
                {
                    foreach (var binding in bindings)
                    {
                        if (binding.StartsWith(bindingStore.Symbol()))
                        {
                            bindingStore.Process(elem, GetKey(binding));
                        }
                    }
                }
            });
            foreach (var bindingStore in _bindingTextStores)
            {
                bindingStore.PostBindingCallback();
            }
        }

        private void ParsePath()
        {
            _rootVisualElement.Query<BindableElement>().ForEach(element =>
            {
                if (string.IsNullOrEmpty(element.bindingPath)) return;

                var bindings = element.bindingPath.Split('.');
                foreach (var bindingStore in _bindingPathStores)
                {
                    foreach (var binding in bindings)
                    {
                        if (binding.StartsWith(bindingStore.Symbol()))
                        {
                            bindingStore.Process(element, GetKey(binding));
                        }
                    }
                }
            });

            foreach (var bindingStore in _bindingPathStores)
            {
                bindingStore.PostBindingCallback();
            }
        }


        private static string GetKey(string str) => str.Substring(1, str.Length - 1);

        public void Dispose()
        {
            foreach (var bindingStore in _bindingTextStores)
            {
                bindingStore.Dispose();
            }

            _bindingTextStores.Clear();

            foreach (var bindingStore in _bindingPathStores)
            {
                bindingStore.Dispose();
            }

            _bindingPathStores.Clear();
        }
    }
}