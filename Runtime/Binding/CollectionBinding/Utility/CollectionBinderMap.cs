using System;
using System.Collections.Generic;
using MVVMToolkit.Binding.CollectionBinding.Generics;
using MVVMToolkit.TypeSerialization;

namespace MVVMToolkit.Binding.CollectionBinding.Utility
{
    public static class CollectionBinderMap
    {
        private static readonly Dictionary<Type, Type> Map;

        static CollectionBinderMap()
        {
            Map = new();
            foreach (var type in TypeUtility.GetTypes(typeof(ICollectionBinder)))
            {
                var instance = (ICollectionBinder)Activator.CreateInstance(type);
                Map.Add(instance.Type, type);
            }
        }

        public static ICollectionBinder GetBinder(Type viewType) =>
            (ICollectionBinder)Activator.CreateInstance(Map[viewType]);
    }
}