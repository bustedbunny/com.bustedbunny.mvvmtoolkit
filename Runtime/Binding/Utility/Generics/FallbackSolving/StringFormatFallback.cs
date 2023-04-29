using System;
using System.Reflection;

namespace MVVMToolkit.Binding.Generics
{
    public class StringFormatFallback<T> : IStringFormatFallback
    {
        public Action SolveArraySetElement(PropertyInfo propertyInfo, object source, object[] array, int index)
        {
            var getter = (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), source, propertyInfo.GetGetMethod());
            var box = new BoxedValue<T>();
            array[index] = box;

            return () => box.value = getter();
        }
    }

    public interface IStringFormatFallback
    {
        public Action SolveArraySetElement(PropertyInfo propertyInfo, object source, object[] array, int index);
    }
}