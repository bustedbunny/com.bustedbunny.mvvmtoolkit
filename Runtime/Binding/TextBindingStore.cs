using System.Collections.Generic;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding
{
    public abstract class BindingStore<T> : IBindingStore where T : IBindable
    {
        protected readonly object bindingContext;
        protected readonly Dictionary<T, string> boundingMap = new();
        public abstract char Symbol();

        public BindingStore(object binding)
        {
            bindingContext = binding;
        }

        public virtual void PostBindingCallback()
        {
        }

        public abstract void Process(VisualElement element, string key);

        public virtual void Dispose()
        {
            foreach (var (bind, _) in boundingMap)
            {
                bind.Dispose();
            }

            boundingMap.Clear();
        }
    }

    public abstract class TextBindingStore<T> : BindingStore<T> where T : IBindable
    {
        protected TextBindingStore(object binding) : base(binding)
        {
        }
    }

    public interface IBindingStore
    {
        public void PostBindingCallback();
        public void Process(VisualElement element, string key);
        void Dispose();
        char Symbol();
    }

    public interface IBindable
    {
        public void Dispose();
    }
}