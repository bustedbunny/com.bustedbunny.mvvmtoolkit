using System;
using System.ComponentModel;
using System.Diagnostics;
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

            ParsingUtility.GetTargetObject(boundObject, selectors[0], out var target, out var propertyName);
            var commandProperty = PropertyUtility.GetGetProperty(target, propertyName);

            Debug.Assert(commandProperty != null, nameof(commandProperty) + " != null");
            Command = (IRelayCommand)commandProperty.GetValue(target);

            if (selectors.Length == 2)
            {
                _argument = ParseArgument(selectors[1]);
            }


            if (element is Button button)
            {
                button.clicked += OnClick;
            }
            else
            {
                element.RegisterCallback<ClickEvent>(OnClick);
            }

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

        private void OnClick() => Command.Execute(_argument);
        private void OnClick(ClickEvent evt) => OnClick();

        private void SetCanExecute(object sender, EventArgs eventArgs)
        {
            Element.SetEnabled(Command.CanExecute(_argument));
        }


        public void Unbind()
        {
            Command.CanExecuteChanged -= SetCanExecute;
            if (Element is Button button)
            {
                button.clicked -= OnClick;
            }
            else
            {
                Element.UnregisterCallback<ClickEvent>(OnClick);
            }
        }
    }
}