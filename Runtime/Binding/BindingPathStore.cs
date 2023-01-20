namespace MVVMToolkit.Binding
{
    public abstract class BindingPathStore<T> : BindingStore<T> where T : IBindable
    {
        protected BindingPathStore(object binding) : base(binding)
        {
        }
    }
}