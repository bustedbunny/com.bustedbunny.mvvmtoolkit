using System.Diagnostics;
using System.Reflection;

namespace MVVMToolkit.Binding
{
    public static class BindingUtility
    {
        public static void GetTargetObject(object root, string key, out object target, out string propertyName)
        {
            Throw.ThrowNullOrEmpty(key);
            target = root;
            var paths = key.Split('.');
            if (paths.Length == 1)
            {
                propertyName = paths[0];
                return;
            }

            for (int i = 0; i < paths.Length - 1; i++)
            {
                var path = paths[i];
                var type = target.GetType();
                var nestedProperty = type.GetProperty(path,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
                Debug.Assert(nestedProperty != null, $"Nested property getter is null for type {type.Name}");
                target = nestedProperty.GetValue(target);
                Debug.Assert(root != null, $"Obtained nested object is null in type {type.Name}");
            }

            propertyName = paths[^1];
        }
    }
}