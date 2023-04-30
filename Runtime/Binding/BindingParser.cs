using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MVVMToolkit.Binding.Custom;
using MVVMToolkit.Binding.Localization;
using MVVMToolkit.Binding.Tooltips;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding
{
    public class BindingParser : BaseParser<IBindingParser>
    {
        private readonly VisualElement _rootVisualElement;


        public BindingParser(INotifyPropertyChanged bindingContext, VisualElement root,
            LocalizedStringTable[] stringTables,
            LocalizedAssetTable[] assetTables) : base(bindingContext)
        {
            _rootVisualElement = root;

            if (stringTables.Length > 0)
            {
                // UnsafeAs allows to skip some checks
                TextStores.Add(new LocalizationTextParser(Binding, stringTables,
                    static (element, s) => Unsafe.As<TextElement>(element).text = s));
                TooltipStores.Add(new TooltipLocalizationParser(Binding, stringTables,
                    TooltipUtility.TooltipBindingOperation));
            }

            TextStores.Add(new StringFormatParser(Binding, static (element, s) => ((TextElement)element).text = s));
            TooltipStores.Add(new TooltipFormatParser(Binding, TooltipUtility.TooltipBindingOperation));

            ViewDataKeyStores.Add(new ClickParser(Binding));
            ViewDataKeyStores.Add(new ValueChangedParser(Binding));
            ViewDataKeyStores.Add(new ReflectionParser(Binding));
            ViewDataKeyStores.Add(new LocalizationAssetParser(assetTables));

            ViewDataKeyStores.Add(new CustomBindingParser(Binding));

            ParseBindings();
        }


        private void ParseBindings()
        {
            ParseAll(_rootVisualElement, item =>
            {
                Parse(item, TextStores, textGetter);
                Parse(item, TooltipStores, tooltipGetter);
                ParseMultiple(item, ViewDataKeyStores, viewDataKeyGetter);
            });
        }

        private static void ParseAll(VisualElement root, Action<VisualElement> funcCall)
        {
            funcCall(root);
            foreach (var element in root.Children())
            {
                ParseAll(element, funcCall);
            }
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
                            Debug.LogError($"Key that caused error: {binding}. Binding type: {Binding.GetType()}");
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
                        Debug.LogError($"Key that caused error: {key}. Binding type: {Binding.GetType()}");
                        Debug.LogException(e);
                    }
                }
            }
        }
    }
}