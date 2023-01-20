using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Entities;

namespace MVPToolkit.Editor.PropertyWindow
{
    public static class PropertyAnalyzer
    {
        private static IEnumerable<string> _except;

        private static IEnumerable<string> Except => _except ??= GetExcept();

        private static IEnumerable<string> GetExcept()
        {
            foreach (var propertyInfo in typeof(SystemBase).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (propertyInfo.CanRead) yield return propertyInfo.Name;
            }
        }

        public static IEnumerable<PropertyInfo> GetValidProperties(Type type)
        {
            var except = Except;
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                         .Where(x => x.CanRead && !except.Contains(x.Name)))
            {
                if (propertyInfo.CanRead) yield return propertyInfo;
            }
        }
    }
}