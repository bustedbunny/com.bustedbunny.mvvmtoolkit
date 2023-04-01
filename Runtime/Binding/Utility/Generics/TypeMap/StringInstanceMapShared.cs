using System;
using System.Collections.Generic;
using MVVMToolkit.TypeSerialization;

namespace MVVMToolkit.Binding.Generics
{
    public abstract class StringInstanceMapShared<T>
    {
        private static readonly Dictionary<string, T> Map;

        static StringInstanceMapShared()
        {
            Map = new();
            foreach (var type in TypeUtility.GetTypes(typeof(T)))
            {
                Map.Add(type.Name, (T)Activator.CreateInstance(type));
            }
        }

        public static T GetInstance(string key) => Map[key];
    }
}