using System;
using System.Collections.Generic;
using System.ComponentModel;
using MVVMToolkit.Binding.Localization.Source;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace MVVMToolkit.Binding.Localization
{
    public class LocalizedStringBinding : IDisposable
    {
        private readonly LocalizedString _ls;
        private readonly BindingGroup _rootBinding;

        public LocalizedStringBinding(LocalizedString ls, INotifyPropertyChanged binding)
        {
            _ls = ls;
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChange;

            _rootBinding = new(binding);

            _ls.Arguments ??= new List<object>();
            if (!_ls.Arguments.Contains(_rootBinding))
                _ls.Arguments.Add(_rootBinding);
        }

        private void OnLocaleChange(Locale obj)
        {
            _rootBinding.ClearBindings();
        }

        private void CleanUpBindings()
        {
            _rootBinding.ClearBindings();
            _ls?.Arguments?.Remove(_rootBinding);
        }

        public void Dispose()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChange;
            CleanUpBindings();
        }
    }
}