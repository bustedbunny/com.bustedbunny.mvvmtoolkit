namespace MVVMToolkit.Binding.Generics
{
    public class BoxedValue<T>
    {
        public T value;

        public override string ToString() => value?.ToString() ?? "null";
    }
}