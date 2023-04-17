﻿using System.Collections.Generic;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.Input;
using MVVMToolkit.Binding.CollectionBinding.Generics;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.CollectionBinding
{
    [Preserve]
    public class ListViewBinder : CollectionBinder<ListView>
    {
        protected override void BindCollection()
        {
            Collection.makeItem = DataTemplate.Instantiate;
            Collection.bindItem = CollectionBindItem;
            Collection.unbindItem = CollectionUnbindItem;
            Collection.itemsSource = Data;
            Notifier.CollectionChanged += OnCollectionChanged;

            if (Command is IRelayCommand<IEnumerable<int>> onIndices)
            {
                Collection.selectedIndicesChanged += onIndices.Execute;
            }
            else if (Command is IRelayCommand<IEnumerable<object>> onChange)
            {
                Collection.selectionChanged += onChange.Execute;
            }
        }

        protected override void UnbindCollection()
        {
            Collection.itemsSource = null;
            Collection.makeItem = null;
            Collection.bindItem = null;
            Collection.unbindItem = null;
            Notifier.CollectionChanged -= OnCollectionChanged;

            if (Command is IRelayCommand<IEnumerable<int>> onIndices)
            {
                Collection.selectedIndicesChanged -= onIndices.Execute;
            }
            else if (Command is IRelayCommand<IEnumerable<object>> onChange)
            {
                Collection.selectionChanged -= onChange.Execute;
            }
        }

        private void CollectionBindItem(VisualElement element, int i)
        {
            var runtimeTemplate = (RuntimeTemplate)element;
            var item = Collection.itemsSource[i];
            foreach (var (child, binder) in runtimeTemplate.bindings)
            {
                binder.Bind(item, child);
            }
        }

        private void CollectionUnbindItem(VisualElement element, int i)
        {
            var runtimeTemplate = (RuntimeTemplate)element;
            foreach (var (child, binder) in runtimeTemplate.bindings)
            {
                binder.Unbind(child);
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action is NotifyCollectionChangedAction.Add)
            {
                Collection.RefreshItems();
                // Collection.RefreshItem(e.NewStartingIndex);
            }
            else if (e.Action is NotifyCollectionChangedAction.Remove)
            {
                Collection.RefreshItems();
                // Collection.RefreshItem(e.OldStartingIndex);
            }
            else if (e.Action is NotifyCollectionChangedAction.Move)
            {
                Collection.RefreshItem(e.OldStartingIndex);
                Collection.RefreshItem(e.NewStartingIndex);
            }
            else if (e.Action is NotifyCollectionChangedAction.Replace)
            {
                Collection.RefreshItem(e.OldStartingIndex);
            }
            else if (e.Action is NotifyCollectionChangedAction.Reset)
            {
                Collection.RefreshItems();
            }
        }
    }
}