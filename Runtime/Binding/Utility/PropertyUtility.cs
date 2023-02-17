using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

// ReSharper disable UnusedMember.Global

namespace MVVMToolkit.Binding
{
    public static class PropertyUtility
    {
        [return: NotNull]
        public static PropertyInfo GetGetProperty(object source, string propertyName)
        {
            return GetGetProperty(source.GetType(), propertyName);
        }

        [return: NotNull]
        public static PropertyInfo GetGetProperty(Type type, string propertyName)
        {
            const BindingFlags flags = BindingFlags.Instance |
                                       BindingFlags.Public |
                                       BindingFlags.NonPublic |
                                       BindingFlags.GetProperty;
            return GetProperty(type, propertyName, flags);
        }

        [return: NotNull]
        public static PropertyInfo GetSetProperty(object source, string propertyName)
        {
            return GetSetProperty(source.GetType(), propertyName);
        }

        [return: NotNull]
        public static PropertyInfo GetSetProperty(Type type, string propertyName)
        {
            const BindingFlags flags = BindingFlags.Instance |
                                       BindingFlags.Public |
                                       BindingFlags.NonPublic |
                                       BindingFlags.SetProperty;
            return GetProperty(type, propertyName, flags);
        }

        [return: NotNull]
        public static PropertyInfo GetGetSetProperty(object source, string propertyName)
        {
            return GetGetSetProperty(source.GetType(), propertyName);
        }

        [return: NotNull]
        public static PropertyInfo GetGetSetProperty(Type type, string propertyName)
        {
            const BindingFlags flags = BindingFlags.Instance |
                                       BindingFlags.Public |
                                       BindingFlags.NonPublic |
                                       BindingFlags.GetProperty |
                                       BindingFlags.SetProperty;
            return GetProperty(type, propertyName, flags);
        }

        [return: NotNull]
        private static PropertyInfo GetProperty(Type type, string propertyName, BindingFlags flags)
        {
            var property = type.GetProperty(propertyName, flags);
            if (property is null)
            {
                // Fallback if property is explicitly declared as interface
                var properties = type.GetProperties(flags);
                foreach (var propertyInfo in properties)
                {
                    if (propertyInfo.Name.EndsWith(propertyName))
                    {
                        return propertyInfo;
                    }
                }

                throw new BindingException($"Type {type.Name} has no property of name {propertyName}.");
            }

            return property;
        }
    }
}