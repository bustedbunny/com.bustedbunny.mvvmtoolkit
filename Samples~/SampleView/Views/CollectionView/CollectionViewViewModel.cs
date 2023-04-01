using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MVVMToolkit;
using MVVMToolkit.Binding.CollectionBinding.Generics;
using MVVMToolkit.Binding.Custom;
using MVVMToolkit.Messaging;
using SampleView.TestView;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace SampleView.CollectionView
{
    public partial class CollectionViewViewModel : ViewModel
    {
        // We declare serialized field so we can fill our collection with some data defined in inspector
        [SerializeField] private MyData[] _initialData;

        protected override void OnInit()
        {
            // Simply add all elements to collection at any point of execution
            foreach (var myData in _initialData)
            {
                Collection.Add(myData);
            }
        }

        // Collection must be a property
        public ObservableCollection<MyData> Collection { get; } = new();

        private void OnSelected(IEnumerable<object> obj)
        {
            foreach (MyData data in obj)
            {
                Debug.Log($"Selected {data.text}");
            }
        }

        // This class is a custom binder that serves purpose to subscribe to `selectionChanged` event on ListView
        // Name of class is important as type name declared in UXML must match it - {*CollectionViewBinder}
        [Preserve]
        private class CollectionViewBinder : CustomBinder<ListView, CollectionViewViewModel>
        {
            protected override void BindElement() => Element.selectionChanged += BindingContext.OnSelected;

            protected override void UnbindElement() => Element.selectionChanged -= BindingContext.OnSelected;
        }

        // This is simple example of data class. It can be anything, even primitives.
        [Serializable]
        public class MyData
        {
            public string text;
            public Texture2D image;

            // Next two classes will be used in ListView.bindItem and ListView.unbindItem delegates of this example
            // Thus it explicitly declares how element is bound
            // Binder is queried with - {:LabelBinder}
            private class LabelBinder : ItemCollectionBinder<MyData, Label>
            {
                protected override void Bind(MyData item, Label element)
                {
                    element.text = item.text;
                }
            }

            // Binder is queried with - {:ImageBinder}
            private class ImageBinder : ItemCollectionBinder<MyData, VisualElement>
            {
                protected override void Bind(MyData item, VisualElement element)
                {
                    element.style.backgroundImage = item.image;
                }
            }
        }

        [RelayCommand]
        private void OpenTestView()
        {
            Messenger.Send(new CloseViewsMessage());
            Messenger.Send(new OpenTestViewMessage());
        }
    }
}