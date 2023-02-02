using System;
using System.Collections.Generic;
using System.ComponentModel;
using MVVMToolkit.Binding.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding
{
    public class BindingParser : IDisposable
    {
        private readonly VisualElement _rootVisualElement;

        private readonly List<List<IBindingParser>> _stores = new();

        private readonly List<IBindingParser> _textStores = new();
        private readonly List<IBindingParser> _tooltipStores = new();
        private readonly List<IBindingParser> _viewDataKeyStores = new();

        public BindingParser(INotifyPropertyChanged model, VisualElement root, LocalizedStringTable[] tables)
        {
            _stores.Add(_textStores);
            _stores.Add(_tooltipStores);
            _stores.Add(_viewDataKeyStores);

            _rootVisualElement = root;

            if (tables.Length > 0)
            {
                _textStores.Add(new LocalizationParser(model, tables,
                    static (element, s) => ((TextElement)element).text = s));

                // _tooltipStores.Add(new LocalizationParser(model, table,
                //     static (element, s) => element.tooltip = s));
            }

            _textStores.Add(new StringFormatParser(model, static (element, s) => ((TextElement)element).text = s));
            // _tooltipStores.Add(new StringFormatParser(model, static (element, s) => element.tooltip = s));

            _viewDataKeyStores.Add(new ClickParser(model));
            _viewDataKeyStores.Add(new ValueChangedParser(model));
            _viewDataKeyStores.Add(new ReflectionParser(model));

            ParseBindings();
        }

        private void ParseBindings()
        {
            Parse<TextElement>(_textStores, static element => element.text);
            Parse<VisualElement>(_tooltipStores, static element => element.tooltip);
            ParseMultiple<VisualElement>(_viewDataKeyStores, static element => element.viewDataKey);
        }

        private void ParseMultiple<T>(List<IBindingParser> stores, Func<T, string> keyGetter) where T : VisualElement
        {
            _rootVisualElement.Query<T>().ForEach(element =>
            {
                var key = keyGetter(element);
                if (string.IsNullOrEmpty(key)) return;

                var bindings = BindingUtility.GetFormatKeys(key);

                if (bindings is null) return;

                foreach (var binding in bindings)
                {
                    foreach (var store in stores)
                    {
                        if (binding.StartsWith(store.Symbol()))
                        {
                            try
                            {
                                store.Process(element, binding[1..]);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError($"Key that caused error: {binding[1..]}");
                                Debug.LogException(e);
                            }
                        }
                    }
                }
            });
            PostBindingCallback(stores);
        }


        private void Parse<T>(List<IBindingParser> stores, Func<T, string> keyGetter) where T : VisualElement
        {
            _rootVisualElement.Query<T>().ForEach(element =>
            {
                var key = keyGetter(element);
                if (string.IsNullOrEmpty(key)) return;
                foreach (var store in stores)
                {
                    if (key.StartsWith(store.Symbol()))
                    {
                        try
                        {
                            store.Process(element, key[1..]);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Key that caused error: {key}");
                            Debug.LogException(e);
                        }
                    }
                }
            });
            PostBindingCallback(stores);
        }

        private void PostBindingCallback(List<IBindingParser> stores)
        {
            foreach (var store in stores)
            {
                store.PostBindingCallback();
            }
        }

        public void Dispose()
        {
            foreach (var stores in _stores)
            {
                foreach (var bindingStore in stores)
                {
                    bindingStore.Dispose();
                }
            }
        }
    }
}