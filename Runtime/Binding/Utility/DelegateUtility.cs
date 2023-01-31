using System;
using System.Reflection;

namespace MVVMToolkit.Binding
{
    public static class DelegateUtility
    {
        public static Func<object> GenerateGetter(object target, string propertyName)
        {
            var type = target.GetType();
            var property = type.GetProperty(propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty);
            if (property is null)
                throw new BindingException($"Can't get property: \"{propertyName}\" in type: \"{type.Name}\"");
            return Get(property, target);
        }


        public static Func<object> Get(PropertyInfo pi, object target)
        {
            var mi = pi.GetGetMethod();
            var type = pi.PropertyType;
            if (type == typeof(int))
                return ReturnGetFunc<int>(mi, target);

            if (type == typeof(uint))
                return ReturnGetFunc<uint>(mi, target);

            if (type == typeof(byte))
                return ReturnGetFunc<byte>(mi, target);

            if (type == typeof(float))
                return ReturnGetFunc<float>(mi, target);

            if (type == typeof(double))

                return ReturnGetFunc<double>(mi, target);

            if (type == typeof(string))

                return ReturnGetFunc<string>(mi, target);

            if (type == typeof(bool))

                return ReturnGetFunc<bool>(mi, target);

            return () => pi.GetValue(target);
        }

        private static Func<object> ReturnGetFunc<T>(MethodInfo mi, object target)
        {
            var func = (Func<T>)mi.CreateDelegate(typeof(Func<T>), target);
            return () => func();
        }
    }
}