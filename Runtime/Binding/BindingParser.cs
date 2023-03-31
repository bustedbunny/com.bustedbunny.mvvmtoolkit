using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MVVMToolkit.Binding.Localization;
using MVVMToolkit.Binding.Tooltips;
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
        private readonly INotifyPropertyChanged _binding;

        public BindingParser(INotifyPropertyChanged model, VisualElement root, LocalizedStringTable[] stringTables,
            LocalizedAssetTable[] assetTables)
        {
            _binding = model;

            _stores.Add(_textStores);
            _stores.Add(_tooltipStores);
            _stores.Add(_viewDataKeyStores);

            _rootVisualElement = root;

            if (stringTables.Length > 0)
            {
                // UnsafeAs allows to skip some checks
                _textStores.Add(new LocalizationTextParser(model, stringTables,
                    static (element, s) => Unsafe.As<TextElement>(element).text = s));
                _tooltipStores.Add(new TooltipLocalizationParser(model, stringTables,
                    TooltipUtility.TooltipBindingOperation));
            }

            _textStores.Add(new StringFormatParser(model, static (element, s) => ((TextElement)element).text = s));
            _tooltipStores.Add(new TooltipFormatParser(model, TooltipUtility.TooltipBindingOperation));

            _viewDataKeyStores.Add(new ClickParser(model));
            _viewDataKeyStores.Add(new ValueChangedParser(model));
            _viewDataKeyStores.Add(new ReflectionParser(model));
            _viewDataKeyStores.Add(new LocalizationAssetParser(assetTables));

            ParseBindings();
        }


        private void ParseBindings()
        {
            _rootVisualElement.Query().ForEach(item =>
            {
                Parse<TextElement>(item, _textStores, static element => element.text);
                Parse<VisualElement>(item, _tooltipStores, static element => element.tooltip);
                ParseMultiple<VisualElement>(item, _viewDataKeyStores, static element => element.viewDataKey);
            });
        }

        private void ParseMultiple<T>(VisualElement item, List<IBindingParser> stores, Func<T, string> keyGetter)
            where T : VisualElement
        {
            if (item is not T target)
            {
                return;
            }

            var key = keyGetter(target);
            if (string.IsNullOrEmpty(key)) return;

            var bindings = ParsingUtility.GetFormatKeys(key);

            if (bindings is null) return;

            foreach (var binding in bindings)
            {
                foreach (var store in stores)
                {
                    if (binding.StartsWith(store.Symbol()))
                    {
                        try
                        {
                            store.Process(target, binding[1..]);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Key that caused error: {binding}. Binding type: {_binding.GetType()}");
                            Debug.LogException(e);
                        }
                    }
                }
            }
        }


        private void Parse<T>(VisualElement item, List<IBindingParser> stores, Func<T, string> keyGetter)
            where T : VisualElement
        {
            if (item is not T target)
            {
                return;
            }

            var key = keyGetter(target);
            if (string.IsNullOrEmpty(key)) return;
            foreach (var store in stores)
            {
                if (key.StartsWith(store.Symbol()))
                {
                    try
                    {
                        store.Process(target, key[1..]);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Key that caused error: {key}. Binding type: {_binding.GetType()}");
                        Debug.LogException(e);
                    }
                }
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