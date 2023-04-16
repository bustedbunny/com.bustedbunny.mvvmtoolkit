using System;
using System.ComponentModel;
using MVVMToolkit.Binding.Localization.Source;
using UnityEngine.Localization;

namespace MVVMToolkit.Binding.Localization
{
    public abstract class BaseLocalizedTextBinding : IElementBinding
    {
        public abstract void Unbind();
    }

    public class LocalizedTextBinding<T> : BaseLocalizedTextBinding
    {
        private readonly T _boundElement;
        private readonly Action<T, string> _operation;
        private readonly BindingGroup _rootBinding;

        public LocalizedTextBinding(T boundElement, INotifyPropertyChanged binding, LocalizedString ls,
            Action<T, string> operation)
        {
            _operation = operation;
            _boundElement = boundElement;
            _rootBinding = new(binding, ls);
            ls.StringChanged += StringChanged;
            ls.GetLocalizedStringAsync().Completed += handle => { StringChanged(handle.Result); };
        }

        private void StringChanged(string value) => _operation(_boundElement, value);

        public override void Unbind()
        {
            _rootBinding.Dispose();
        }
    }
}