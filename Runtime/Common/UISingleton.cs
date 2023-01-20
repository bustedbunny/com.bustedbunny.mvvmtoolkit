using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging;
using Unity.Entities;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace MVVMToolkit
{
    public class UISingleton : IComponentData
    {
        public IMessenger Messenger { get; set; }
        private UIDocument _document;

        public UIDocument Document
        {
            get => _document;
            set
            {
                if (_document == value) return;

                // If old document is not null we clear it from our presentations
                if (_document is not null)
                {
                    foreach (var viewModel in activeViewModels)
                    {
                        AddToPanel(viewModel.RootVisualElement);
                    }

                    Sort();
                }

                // If new document is not null we add all our active presentations
                if (value is not null)
                {
                    foreach (var viewModel in activeViewModels)
                    {
                        RemoveFromPanel(viewModel.RootVisualElement);
                    }
                }

                _document = value;
            }
        }

        public void Add(BaseViewModel viewModel)
        {
            Assert.IsFalse(activeViewModels.Contains(viewModel));
            activeViewModels.Add(viewModel);
            AddToPanel(viewModel.RootVisualElement);
            Sort();
        }

        public void Remove(BaseViewModel viewModel)
        {
            Assert.IsTrue(activeViewModels.Contains(viewModel));
            activeViewModels.Remove(viewModel);
            RemoveFromPanel(viewModel.RootVisualElement);
        }

        public void Clear()
        {
            foreach (var viewModel in activeViewModels)
            {
                RemoveFromPanel(viewModel.RootVisualElement);
            }

            activeViewModels.Clear();
        }

        private void AddToPanel(VisualElement view)
        {
            _document?.rootVisualElement.Add(view);
        }

        private void RemoveFromPanel(VisualElement view)
        {
            _document?.rootVisualElement.Remove(view);
        }

        private void Sort()
        {
            _document?.rootVisualElement.Sort(Comparison);
        }

        private static int Comparison(VisualElement x, VisualElement y)
        {
            if (x is OrderedContainer orderedX && y is OrderedContainer orderedY)
            {
                return Comparer<int>.Default.Compare(orderedX.Order, orderedY.Order);
            }
            else
            {
                return 0;
            }
        }


        public readonly List<BaseViewModel> viewModels = new();
        public readonly HashSet<BaseViewModel> activeViewModels = new();
    }
}