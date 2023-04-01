using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.CollectionBinding.Generics
{
    public abstract class ItemCollectionBinder<TItem, TElement> : IItemCollectionBinder where TElement : VisualElement
    {
        protected abstract void Bind(TItem item, TElement element);

        protected virtual void Unbind(TElement element) { }

        public void Bind(object item, VisualElement element) => Bind((TItem)item, (TElement)element);

        public void Unbind(VisualElement element) => Unbind((TElement)element);
    }

    public interface IItemCollectionBinder
    {
        public void Bind(object item, VisualElement element);
        public void Unbind(VisualElement element);
    }
}