using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging;
using MVVMToolkit.DependencyInjection;
using UnityEngine;
using UnityEngine.UIElements;

namespace MVVMToolkit
{
    public class UIRoot : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;

        private UIDocument _uiDocument;

        public UIDocument UIDocument
        {
            get => _uiDocument;
            set
            {
                if (_uiDocument != value)
                {
                    value?.rootVisualElement.Add(Root);
                    _uiDocument?.rootVisualElement.Remove(Root);
                    _uiDocument = value;
                }
            }
        }

        public VisualElement Root { get; private set; }

        private readonly List<BaseView> _views = new();
        private readonly List<ViewModel> _viewModels = new();


        public void Initialize(StrongReferenceMessenger messenger, ServiceProvider serviceProvider)
        {
            if (Root is not null) throw new InvalidOperationException("Cannot initialize UIRoot multiple times");

            Root = new VisualElement { style = { flexGrow = 1f } };
            UIDocument = uiDocument;

            foreach (var viewModel in GetComponentsInChildren<ViewModel>())
            {
                _viewModels.Add(viewModel);
                serviceProvider.RegisterService(viewModel);
            }

            foreach (var view in GetComponentsInChildren<BaseView>()) _views.Add(view);


            // After all services have been registered we do dependency injection
            serviceProvider.Inject();

            // Now we resolve cross references for views and viewModels
            foreach (var view in _views) view.Attach(messenger, serviceProvider);
            foreach (var viewModel in _viewModels) viewModel.Attach(messenger, serviceProvider);


            var sortDict = new Dictionary<VisualElement, int>(_views.Count);

            foreach (var view in _views)
            {
                view.Initialize();
                if (view.SortLayer != 0)
                    sortDict.Add(view.RootVisualElement, view.SortLayer);
            }

            foreach (var view in _views)
            {
                var parent = view.ResolveParent() ?? Root;
                parent.Add(view.RootVisualElement);
                parent.Sort((x, y) =>
                {
                    sortDict.TryGetValue(x, out var xSort);
                    sortDict.TryGetValue(y, out var ySort);
                    return Comparer<int>.Default.Compare(xSort, ySort);
                });
            }
        }
    }
}