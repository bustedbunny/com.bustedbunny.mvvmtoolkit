using System;
using System.Collections;
using System.Collections.Specialized;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.CollectionBinding.Generics
{
    public abstract class CollectionBinder<T> : ICollectionBinder where T : VisualElement
    {
        public Type Type => typeof(T);
        protected T Collection { get; private set; }

        protected IList Data { get; private set; }
        protected INotifyCollectionChanged Notifier { get; private set; }
        protected DataTemplate DataTemplate { get; private set; }

        protected abstract void BindCollection();
        protected abstract void UnbindCollection();


        public void Bind(VisualElement view, IList data, DataTemplate template)
        {
            if (data is not INotifyCollectionChanged notifier)
            {
                throw new BindingException(
                    $"{nameof(data)} does not implement {nameof(INotifyCollectionChanged)}");
            }

            Data = data;
            Notifier = notifier;
            Collection = (T)view;
            DataTemplate = template;
            BindCollection();
        }

        public void Unbind()
        {
            UnbindCollection();
            Data = null;
            Notifier = null;
            Collection = null;
        }
    }

    public interface ICollectionBinder : IElementBinding
    {
        public Type Type { get; }
        public void Bind(VisualElement view, IList data, DataTemplate template);
    }
}