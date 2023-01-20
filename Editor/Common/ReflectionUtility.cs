using System;
using System.Collections.Generic;
using MVVMToolkit;

namespace MVPToolkit.Editor.Common
{
    public class ReflectionUtility
    {
        public static Type[] GetPresentations()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var types = new List<Type>();
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(ViewModel).IsAssignableFrom(type) && !type.IsAbstract)
                    {
                        types.Add(type);
                    }
                }
            }

            return types.ToArray();
        }
    }
}