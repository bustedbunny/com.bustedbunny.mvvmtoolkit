using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using CommunityToolkit.Mvvm.Input;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding
{
    public class ClickParser : BindingParser<CommandBinding>
    {
        public ClickParser(INotifyPropertyChanged viewModel) : base(viewModel) { }

        public override char Symbol() => '@';

        public override void Process(VisualElement element, string key)
        {
            boundingMap.Add(new(element, bindingContext, key), key);
        }
    }

    public class CommandBinding : IElementBinding
    {
        private IRelayCommand Command { get; }
        private readonly object _argument;
        private VisualElement Element { get; }

        public CommandBinding(VisualElement element, object boundObject, string key)
        {
            Throw.ThrowNullOrEmpty(key);


            var selectors = key.Split(':');
            if (selectors.Length > 2) throw new($"Invalid command arguments. Key: {key}");

            Element = element;

            BindingUtility.GetTargetObject(boundObject, selectors[0], out var target, out var propertyName);

            var boundType = target.GetType();
            var commandProperty = boundType.GetProperty(propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

            Debug.Assert(commandProperty != null, nameof(commandProperty) + " != null");
            Command = (IRelayCommand)commandProperty.GetValue(target);

            if (selectors.Length == 2)
            {
                _argument = ParseArgument(selectors[1]);
            }


            element.RegisterCallback<ClickEvent>(OnClick);
            Command.CanExecuteChanged += SetCanExecute;

            SetCanExecute(null, null);
        }

        private static object ParseArgument(string key)
        {
            if (bool.TryParse(key, out var boolResult)) return boolResult;
            if (int.TryParse(key, out var intResult)) return intResult;
            if (float.TryParse(key, out var floatResult)) return floatResult;
            return key;
        }

        private void OnClick(ClickEvent evt) => Command.Execute(_argument);

        private void SetCanExecute(object sender, EventArgs eventArgs)
        {
            Element.SetEnabled(Command.CanExecute(_argument));
        }


        public void Dispose()
        {
            Command.CanExecuteChanged -= SetCanExecute;
            Element.UnregisterCallback<ClickEvent>(OnClick);
        }
    }
}