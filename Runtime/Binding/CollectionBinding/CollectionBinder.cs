namespace MVVMToolkit.Binding.CollectionBinding
{
    public abstract class CollectionBinder<TItem, TElement> : ICollectionBinder
    {
        protected abstract void Bind(TItem item, TElement element);

        protected virtual void Unbind(TItem element) { }

        public void Bind(object item, object element)
        {
            Bind((TItem)item, (TElement)element);
        }

        public void Unbind(object element)
        {
            Unbind((TItem)element);
        }
    }

    public interface ICollectionBinder
    {
        public void Bind(object item, object element);
        public void Unbind(object element);
    }
}