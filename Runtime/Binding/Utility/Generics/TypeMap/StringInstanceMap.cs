using System;
using System.Collections.Generic;
using MVVMToolkit.TypeSerialization;

namespace MVVMToolkit.Binding.Generics
{
    public abstract class StringInstanceMap<T>
    {
        private static readonly Dictionary<string, Type> Map;

        static StringInstanceMap()
        {
            Map = new();
            foreach (var type in TypeUtility.GetTypes(typeof(T)))
            {
                Map.Add(type.Name, type);
            }
        }

        public static T GetInstance(string key) => (T)Activator.CreateInstance(Map[key]);
    }
}