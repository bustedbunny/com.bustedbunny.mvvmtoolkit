using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding
{
    public abstract class BindingParser<T> : IBindingParser where T : IElementBinding
    {
        protected readonly INotifyPropertyChanged bindingContext;
        protected readonly Dictionary<T, string> boundingMap = new();
        public abstract char Symbol();

        protected BindingParser(INotifyPropertyChanged binding)
        {
            bindingContext = binding;
        }

        public virtual void PostBindingCallback() { }

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


    public interface IBindingParser
    {
        public void PostBindingCallback();
        public void Process(VisualElement element, string key);
        void Dispose();
        char Symbol();
    }

    public interface IElementBinding
    {
        public void Dispose();
    }
}