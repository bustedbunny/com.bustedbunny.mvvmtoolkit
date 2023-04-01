using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.Custom
{
    public abstract class CustomBinder<TElement, TBindingContext> : ICustomBinder where TElement : VisualElement
    {
        protected TElement Element { get; private set; }
        protected TBindingContext BindingContext { get; private set; }

        protected abstract void BindElement();
        protected abstract void UnbindElement();


        public void Bind(VisualElement element, object bindingContext)
        {
            Element = (TElement)element;
            BindingContext = (TBindingContext)bindingContext;
            BindElement();
        }

        public void Unbind()
        {
            UnbindElement();
            Element = null;
            BindingContext = default;
        }
    }

    public interface ICustomBinder : IElementBinding
    {
        public void Bind(VisualElement element, object bindingContext);
    }
}