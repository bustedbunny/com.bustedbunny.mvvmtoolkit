using System.Collections.Specialized;
using MVVMToolkit.Binding.CollectionBinding.Generics;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.CollectionBinding
{
    public class ListViewBinder : CollectionBinder<ListView>
    {
        protected override void BindCollection()
        {
            Collection.makeItem = DataTemplate.Instantiate;
            Collection.bindItem = CollectionBindItem;
            Collection.unbindItem = CollectionUnbindItem;
            Collection.itemsSource = Data;
            Notifier.CollectionChanged += OnCollectionChanged;
            
            
        }

        private void CollectionBindItem(VisualElement element, int i)
        {
            var runtimeTemplate = (RuntimeTemplate)element;
            foreach (var (child, binder) in runtimeTemplate.bindings)
            {
                binder.Bind(Collection.itemsSource[i], child);
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
                Collection.RefreshItem(e.NewStartingIndex);
            }
            else if (e.Action is NotifyCollectionChangedAction.Remove)
            {
                Collection.RefreshItem(e.OldStartingIndex);
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

        protected override void UnbindCollection()
        {
            Collection.itemsSource = null;
            Collection.makeItem = null;
            Collection.bindItem = null;
            Collection.unbindItem = null;
            Notifier.CollectionChanged -= OnCollectionChanged;
        }
    }
}