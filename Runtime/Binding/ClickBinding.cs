using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using CommunityToolkit.Mvvm.Input;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding
{
    public class ClickParser : BindingParser<ClickBinding>
    {
        public ClickParser(INotifyPropertyChanged viewModel) : base(viewModel) { }

        public override char Symbol() => '@';

        public override void Process(VisualElement element, string key)
        {
            boundingMap.Add(new ClickBinding(element, bindingContext, key), key);
        }
    }

    public class ClickBinding : IElementBinding
    {
        public IRelayCommand Command { get; }
        public VisualElement Element { get; }

        public ClickBinding(VisualElement element, object boundObject, string key)
        {
            Throw.ThrowNullOrEmpty(key);
            var selectors = key.Split(':');
            if (selectors.Length > 1) throw new Exception($"Cannot have more than 1 key. Key: {key}");

            BindingUtility.GetTargetObject(boundObject, selectors[0], out var target, out var propertyName);

            var boundType = target.GetType();
            var commandProperty = boundType.GetProperty(propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

            Debug.Assert(commandProperty != null, nameof(commandProperty) + " != null");
            Command = (IRelayCommand)commandProperty.GetValue(target);
            Element = element;

            element.RegisterCallback<ClickEvent>(OnClick);
            Command.CanExecuteChanged += SetCanExecute;

            SetCanExecute(null, null);
        }

        private void OnClick(ClickEvent evt)
        {
            Command.Execute(null);
        }

        private void SetCanExecute(object sender, EventArgs eventArgs)
        {
            Element.SetEnabled(Command.CanExecute(null));
        }


        public void Dispose()
        {
            Command.CanExecuteChanged -= SetCanExecute;
            Element.UnregisterCallback<ClickEvent>(OnClick);
        }
    }
}