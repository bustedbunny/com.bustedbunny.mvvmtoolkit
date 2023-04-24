using System;
using System.ComponentModel;
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
        private readonly IRelayCommand _command;
        private readonly object _argument;
        private VisualElement Element { get; }

        public CommandBinding(VisualElement element, object boundObject, string key)
        {
            Element = element;
            CommandUtility.Parse((INotifyPropertyChanged)boundObject, key, out _command, out _argument);

            if (element is Button button)
            {
                button.clicked += OnClick;
            }
            else
            {
                element.RegisterCallback<ClickEvent>(OnClick);
            }

            _command.CanExecuteChanged += SetCanExecute;

            SetCanExecute(null, null);
        }


        private void OnClick() => _command.Execute(_argument);
        private void OnClick(ClickEvent evt) => OnClick();

        private void SetCanExecute(object sender, EventArgs eventArgs)
        {
            Element.SetEnabled(_command.CanExecute(_argument));
        }


        public void Dispose()
        {
            _command.CanExecuteChanged -= SetCanExecute;
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