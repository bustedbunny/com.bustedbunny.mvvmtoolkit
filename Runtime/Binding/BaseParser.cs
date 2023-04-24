using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding
{
    public abstract class BaseParser<T> : IDisposable where T : IParser
    {
        protected BaseParser(INotifyPropertyChanged bindingContext)
        {
            Binding = bindingContext;
            Stores.Add(TextStores);
            Stores.Add(TooltipStores);
            Stores.Add(ViewDataKeyStores);
        }

        protected readonly Func<TextElement, string> textGetter = static element => element.text;
        protected readonly Func<VisualElement, string> tooltipGetter = static element => element.tooltip;
        protected readonly Func<VisualElement, string> viewDataKeyGetter = static element => element.viewDataKey;
        protected INotifyPropertyChanged Binding { get; }

        protected List<List<T>> Stores { get; } = new();
        protected List<T> TextStores { get; } = new();
        protected List<T> TooltipStores { get; } = new();
        protected List<T> ViewDataKeyStores { get; } = new();

        public virtual void Dispose()
        {
            foreach (var parsers in Stores)
            {
                foreach (var bindingStore in parsers)
                {
                    bindingStore.Dispose();
                }
            }
        }
    }
}