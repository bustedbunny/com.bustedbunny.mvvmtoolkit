using System;
using System.Collections.Generic;
using System.ComponentModel;
using MVVMToolkit.Binding.Localization.Source;
using UnityEngine.Localization;

namespace MVVMToolkit.Binding.Localization
{
    public class LocalizedStringBinding : IDisposable
    {
        private readonly LocalizedString _ls;
        private readonly BindingGroup _rootBinding;

        public LocalizedStringBinding(LocalizedString ls, INotifyPropertyChanged binding)
        {
            _ls = ls;
            _rootBinding = new(binding, ls);

            _ls.Arguments ??= new List<object>();
            if (!_ls.Arguments.Contains(_rootBinding))
                _ls.Arguments.Add(_rootBinding);
        }

        public void Dispose()
        {
            _rootBinding.ClearBindings();
            _ls?.Arguments?.Remove(_rootBinding);
        }
    }
}