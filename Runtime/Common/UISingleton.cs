using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging;
using UnityEngine.UIElements;

namespace MVVMToolkit
{
    public class UISingleton
    {
        public IMessenger Messenger { get; set; }
        private UIDocument _document;

        public UIDocument Document
        {
            get => _document;
            set
            {
                if (_document == value) return;

                if (_document is not null)
                {
                    _document.rootVisualElement.Remove(root);
                }

                value.rootVisualElement.Add(root);

                _document = value;
            }
        }

        public VisualElement root;


        public readonly Dictionary<Type, ViewModel> viewModels = new();
    }
}